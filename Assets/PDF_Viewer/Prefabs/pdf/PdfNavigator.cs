/*using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Jobs;

*/
using UnityEngine;

public class PdfNavigator : MonoBehaviour
{
/*
    const int MAX_RES = 2048;

    string path;
    Document doc;

    int currentPage = 0;

    public Texture2D tex;

    public void Open(string p)
	{
        path = p;

        FileStream fs = new FileStream(path, FileMode.Open);
        doc = new Document(fs, EngineSettings.GlobalSettings);

        tex = new Texture2D(MAX_RES, MAX_RES);
        GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", tex);

        GoToPage(currentPage);

    }
    public void Open(string p, int texX, int texY)
    {
        path = p;

        FileStream fs = new FileStream(path, FileMode.Open);
        doc = new Document(fs, EngineSettings.GlobalSettings);

        tex = new Texture2D(texX, texY);

        GoToPage(currentPage);

    }

    void NextPage() { GoToPage(currentPage + 1); }
    void LastPage() { GoToPage(currentPage - 1); }

    bool pageTurnSemaphore = false;
    void GoToPage(int p)
	{
        if (pageTurnSemaphore) return;
        pageTurnSemaphore = true;

        p = Mathf.Clamp(p, 0, doc.Pages.Count - 1);
        currentPage = p;

        //transform.GetChild(0).GetComponent<TextMeshPro>().text = $"{p+1}/{doc.Pages.Count}";

        Page page = doc.Pages[p];

        PdfUtils.singleton.GetPageAsTexture(page, ref tex, (Texture2D outTex) => { pageTurnSemaphore = false; });
    }



	private void Update()
	{
        //TEMPORARY NAVIGATION CODE
        if (Input.GetKeyDown(KeyCode.D)) NextPage();
        if (Input.GetKeyDown(KeyCode.A)) LastPage();
        pageTurnSemaphore = false;
    }
*/
}