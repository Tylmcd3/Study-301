using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[Elixir]
public class PdfUtils : MonoBehaviour
{
    public static PdfUtils singleton;
	private void Start()
	{
        singleton = this;
        StartCoroutine(ManagePdfReading());
        StartCoroutine(ManageTextureWriting());
    }

	
    const int MAX_TEXTURE_FILL_PER_FRAME = 10000;
    static RenderingSettings renderingSettings = new RenderingSettings();

    Queue<PdfRasterizeInstructions> readQueue = new Queue<PdfRasterizeInstructions>();
    Queue<PdfRasterizeInstructions> writeQueue = new Queue<PdfRasterizeInstructions>();

    public void GetPageAsTexture(Page page, ref Texture2D tex, Action<Texture2D> completeAction)
	{
        readQueue.Enqueue(new PdfRasterizeInstructions() { page = page, textureSize = new Vector2Int(tex.width, tex.height), tex = tex, completeAction = completeAction });
	}
    IEnumerator ManagePdfReading()
    {
        while (true)
		{
            if (readQueue.Count > 0)
            {
                PdfRasterizeInstructions instructions = readQueue.Dequeue();

                Page page = instructions.page;
                Vector2Int textureSize = instructions.textureSize;

                int maxRes = Mathf.Max(textureSize.x, textureSize.y);

                //Calculate resolution
                Vector2 fRes = (maxRes * new Vector2((float)page.Width, (float)page.Height) / Mathf.Max((float)page.Width, (float)page.Height));
                Vector2Int res = new Vector2Int((int)fRes.x, (int)fRes.y);

                //Open a new thread to rasterize the pdf
                byte[] arr = null;
                Thread t = new Thread(() => { arr = page.RenderAsBytes(res.x, res.y, renderingSettings); });
                t.Start();

                //wait until the thread finishes
                while (t.ThreadState != ThreadState.Stopped && t.ThreadState != ThreadState.Aborted)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (t.ThreadState != ThreadState.Stopped)
                {
                    Debug.LogError($"Thread for Changing pdf Page stopped early! Thread state {t.ThreadState}");
                }
                else
				{
                    instructions.arr = arr;
                    instructions.res = res;
                    writeQueue.Enqueue(instructions);
				}
            }
            else yield return new WaitForEndOfFrame();
		}
    }

    IEnumerator ManageTextureWriting()
	{
        while (true)
		{
            if (writeQueue.Count > 0)
			{
                float time = Time.realtimeSinceStartup;

                PdfRasterizeInstructions instructions = writeQueue.Dequeue();

                Vector2Int textureSize = instructions.textureSize;
                Action<Texture2D> completeAction = instructions.completeAction;
                byte[] arr = instructions.arr;
                Vector2Int res = instructions.res;
                Texture2D tex = instructions.tex;
                //tex.Reinitialize(textureSize.x, textureSize.y);


                Vector2Int pageCornerCoords = (textureSize - res) / 2;

                Color[] img = new Color[textureSize.x * textureSize.y];

                for (int i = 0; i < img.Length; i++)
                {
                    int texX = i % textureSize.x, texY = i / textureSize.x;
                    bool inXBounds = texX > pageCornerCoords.x && texX < pageCornerCoords.x + res.x;
                    bool inYBounds = texY > pageCornerCoords.y && texY < pageCornerCoords.y + res.y;

                    if (inXBounds && inYBounds)
					{
                        int pageX = texX - pageCornerCoords.x, pageY = res.y - (texY - pageCornerCoords.y) - 1;
                        int a = (pageX + res.x * pageY)*4;

                        img[i] = new Color(
                        arr[a+2]/127f, //R
                        arr[a+1]/127f, //G
                        arr[a]/127f //B
                        );
					}
                    else
					{
                        img[i] = Color.gray;
                    }

                    //Time management
                    if ((i + 1) % MAX_TEXTURE_FILL_PER_FRAME == 0)
                    {
                        if (time - Time.realtimeSinceStartup > 0.01)
                            Debug.Log(time - Time.realtimeSinceStartup);
                        time = Time.realtimeSinceStartup;
                        yield return new WaitForEndOfFrame();
                    }
                }

                yield return new WaitForEndOfFrame();

                tex.SetPixels(img);
                tex.Apply();

                completeAction(tex);

                yield return new WaitForEndOfFrame();

                GC.Collect();
            }
            yield return new WaitForEndOfFrame();
        }
	}


    struct PdfRasterizeInstructions
	{
        public Page page;
        public Vector2Int textureSize;
        public Vector2Int res;
        public Texture2D tex;
        public Action<Texture2D> completeAction;
        public byte[] arr;
    }


}
