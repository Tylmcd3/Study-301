using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using TMPro;
using UnityEngine;

[Elixir]
public class pdfStorage : MonoBehaviour
{
    public GameObject DocumentPrefab; 
    public GameObject DocumentMenu;
    public int posY = 125;
    public int minPosY = -215;
    public struct DocumentTransfer
    {
        public List<Texture2D> texs;
    }
    public struct Document
    {
        public string Name;
        public List<Texture2D> Pages;
        public List<string> PageName;
    }
    // Start is called before the first frame update
    private List<Document> docs;
    private Document activeDocument;

    Vector2Int pageDimensions;
    void Start()
    {
        
        pageDimensions = GetComponent<DrawingTextureManager>().whiteboardSize;
        string documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pdfFolder = documentsFolderPath + "\\Bonelab\\Documents";
        docs = new List<Document>();

        if (Directory.Exists(pdfFolder))
        {
            string[] subdirectories = Directory.GetDirectories(pdfFolder);

            foreach (string subdirectory in subdirectories)
            {
                LoadDocument(subdirectory);
            }
        }
        else
        {
            Directory.CreateDirectory(pdfFolder);
        }

    }
    private void LoadDocument(string dir)
    {
        Document doc = new Document();
        doc.Name = Path.GetFileName(dir);

        doc.Pages = new List<Texture2D>();
        doc.PageName = new List<string>();

        doc.PageName.AddRange(Directory.GetFiles(dir, "*.png"));
        doc.PageName.AddRange(Directory.GetFiles(dir, "*.jpg"));

        for (int i = 0; i < doc.PageName.Count; i++)
        {
            Texture2D tex = new Texture2D(pageDimensions.x, pageDimensions.y);
            tex = LoadTextureFromFile(doc.PageName[i]);
            doc.Pages.Add(tex);
        }
        if (docs.Count == 0)
        {
            activeDocument = doc;
        }

        docs.Add(doc);
        createUIDocument(doc);
    }
    private void createUIDocument(Document doc)
    {
        
        if(posY >= minPosY)
        {
            GameObject UI = Instantiate(DocumentPrefab);
            UI.name = "DocumentButton" + doc.Name;
            UI.transform.parent = DocumentMenu.transform;
            UI.transform.localPosition = new Vector3(215, posY, 1);
            UI.transform.localScale = Vector3.one;

            posY -= 50;
            buttonCollision col = UI.GetComponent<buttonCollision>();
            col.document = doc.Name;
            col.DocumentGroup = DocumentMenu;
            col.manager = this.gameObject;
            col.toggleGroup = true;

            UI.GetComponentInChildren<TMP_Text>().text = doc.Name;
        }
    }
    private Texture2D LoadTextureFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            // Read the bytes from the file
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);

            // Create a new texture and load the image data
            Texture2D texture = new Texture2D(pageDimensions.x, pageDimensions.y); // You can specify the dimensions as needed
         
            if (texture.LoadImage(imageBytes, false))
            {
                return texture;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public void setActiveDocument(string docName)
    {
        foreach(Document doc in docs)
        {
            if(doc.Name == docName)
            {
                activeDocument = doc;
            }
        }
        GetComponent<DrawingTextureManager>().LoadPDFIntoManager(true);
    }

    public DocumentTransfer getActiveDocument()
    {
        DocumentTransfer doct = new DocumentTransfer();
        doct.texs = activeDocument.Pages;
        return doct;

    }

}
