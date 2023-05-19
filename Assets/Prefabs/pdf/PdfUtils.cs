using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PdfUtils : MonoBehaviour
{
    
    public static PdfUtils singleton;
	private void Start()
	{
        singleton = this;
        renderingSettings.ScaleMode = Apitron.PDF.Rasterizer.Configuration.ScaleMode.PreserveAspectRatio;
        StartCoroutine(ManagePdfReading());
    }

	
    static RenderingSettings renderingSettings = new RenderingSettings();

    Queue<PdfRasterizeInstructions> readQueue = new Queue<PdfRasterizeInstructions>();

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

                //Open a new thread to rasterize the pdf
                byte[] arr = null;
                Thread t = new Thread(() => { 
                    arr = page.RenderAsBytes(textureSize.x, textureSize.y, renderingSettings);

                    int rowLength = textureSize.x * 4;
                    Span<byte> tmp = new Span<byte>(new byte[rowLength]);
                    for (int y = 0; y < textureSize.y/2; y++)
					{
                        int line = y * rowLength;
                        int invLine = arr.Length - ((y + 1) * rowLength);

                        arr.AsSpan().Slice(line, rowLength).CopyTo(tmp);
                        arr.AsSpan().Slice(invLine, rowLength).CopyTo(arr.AsSpan().Slice(line, rowLength));
                        tmp.CopyTo(arr.AsSpan().Slice(invLine, rowLength));
                    }
                });
                t.Start();

                //wait until the thread finishes
                while (t.ThreadState != ThreadState.Stopped && t.ThreadState != ThreadState.Aborted)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (t.ThreadState != ThreadState.Stopped)
                {
                    Debug.LogError($"Thread for Changing pdf Page stopped early! Thread state {t.ThreadState}");
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                instructions.tex.SetPixelData(arr, 0);
                instructions.tex.Apply();
                instructions.completeAction(instructions.tex);
            }
            else yield return new WaitForEndOfFrame();
		}
    }


    struct PdfRasterizeInstructions
	{
        public Page page;
        public Vector2Int textureSize;
        public Texture2D tex;
        public Action<Texture2D> completeAction;
    }


}
