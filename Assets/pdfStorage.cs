using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Elixir]
public class pdfStorage : MonoBehaviour
{
    public List<Texture2D> TempPages;

    public struct pdfStorageStruct
    {
        public string Name;
        public string Dir;
        public Document doc;
        public List<Texture2D> PageCol;
        public Queue<Page> pagesToLoad;
    }
    // Start is called before the first frame update
    public List<pdfStorageStruct> pdfs;

    Vector2Int pageDimensions;
    void Start()
    {
        pageDimensions = GetComponent<DrawingTextureManager>().whiteboardSize;
        string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pdfFolder = documentsFolderPath + "\\Bonelab\\PDFs";
        pdfs = new List<pdfStorageStruct>();
        if (Directory.Exists(pdfFolder))
        {
            string[] pdfFiles = Directory.GetFiles(pdfFolder, "*.pdf");

            foreach (string pdfFile in pdfFiles)
            {
                FileStream fs = new FileStream(pdfFile, FileMode.Open);
                Document doc = new Document(fs, EngineSettings.GlobalSettings);
                Queue<Page> pages = new Queue<Page>();
                foreach (Page page in doc.Pages)
                {
                    pages.Enqueue(page);
                }

                pdfStorageStruct temp = new pdfStorageStruct() { Name = Path.GetFileName(pdfFile), Dir = pdfFile, doc = doc, PageCol = new List<Texture2D>(), pagesToLoad = pages };
                pdfs.Add(temp);
                StartCoroutine(LoadPDFTextures(temp));

            }
        }
        else
        {
            Directory.CreateDirectory(pdfFolder);
        }

       /* pdfs = new List<pdfStorageStruct>();
        pdfStorageStruct temp = new pdfStorageStruct() { Name = "test", Dir = "test", PageCol = TempPages, };
        pdfs.Add(temp);*/

    }

    public List<Texture2D> getPDF()
    {
        if(pdfs.Count > 0)
        {
            return pdfs[0].PageCol;
        }
        return null;
    }

    IEnumerator LoadPDFTextures(pdfStorageStruct pdf)
    {
        while (pdf.pagesToLoad.Count > 0)
        {
            Page page = pdf.pagesToLoad.Dequeue();
            Texture2D tex = new Texture2D(pageDimensions.x, pageDimensions.y);
            pdf.PageCol.Add(tex);
            PdfUtils.singleton.GetPageAsTexture(page, ref tex, (Texture2D outTex) => { });
            yield return new WaitForEndOfFrame();
        }
    }
}
