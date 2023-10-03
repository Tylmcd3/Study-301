/*using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;*/
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[Elixir]
public class pdfStorage : MonoBehaviour
{
    public Texture2D p1;
    public Texture2D p2;
    public Texture2D p3;
    public Texture2D p4;
    public Texture2D p5;
    public List<Texture2D> tempPages;
    public struct Page
    {
        public string Dir;
        public Texture2D PageTex;
    }
    public struct DocumentTransfer
    {
        public List<Texture2D> texs;
    }
    public struct Document
    {
        public string Name;
        public List<Page> PageCol;
    }
    // Start is called before the first frame update
    private List<Document> docs;

    Vector2Int pageDimensions;
    void Start()
    {
        tempPages = new List<Texture2D>();
        tempPages.Add(p1);
        tempPages.Add(p2);
        tempPages.Add(p3);        
        tempPages.Add(p4);
        tempPages.Add(p5);
        pageDimensions = GetComponent<DrawingTextureManager>().whiteboardSize;
        string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pdfFolder = documentsFolderPath + "\\Bonelab\\Documents";
        docs = new List<Document>();

        if (Directory.Exists(pdfFolder))
        {
            string[] subdirectories = Directory.GetDirectories(pdfFolder);

            foreach (string subdirectory in subdirectories)
            {
                /*Document doc = new Document();
                doc.Name = Path.GetDirectoryName(subdirectory);
                doc.PageCol = new List<Page>();
                List<string> images = new List<string>();
                images.AddRange(Directory.GetFiles(subdirectory, "*.png"));
                images.AddRange(Directory.GetFiles(subdirectory, "*.jpg"));*/

                /*foreach(string image in images)
                {
                    Page page = new Page();
                    page.Dir = image;
                    page.PageTex = LoadTextureFromFile(image);
                    doc.PageCol.Add(page);
                }
                docs.Add(doc);*/
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
    private Texture2D LoadTextureFromFile(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            // Read the bytes from the file
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);

            // Create a new texture and load the image data
            Texture2D texture = new Texture2D(pageDimensions.x, pageDimensions.y); // You can specify the dimensions as needed
            if (texture.LoadImage(imageBytes))
            {
                return texture;
            }
            else
            {
                Debug.LogError("Failed to load image data.");
                return null;
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }



    public DocumentTransfer getPDF()
    {
        DocumentTransfer doct = new DocumentTransfer();
        //List<Texture2D> texs = new List<Texture2D>();

        /*if (docs.Count > 0)
        {
            foreach(Page page in docs[0].PageCol)
            {
                //texs.Add(page.PageTex);
            }
        }*/
        doct.texs = tempPages;
        return doct;

    }

}
