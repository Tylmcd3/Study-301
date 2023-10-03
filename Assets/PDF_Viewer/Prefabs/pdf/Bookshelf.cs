/*using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Bookshelf : MonoBehaviour
{

    string bookCollectionPath = "Assets\\PDF_Viewer\\ExamplePDFLibrary";
    public GameObject bookPrefab;

    void Start()
    {
        int i = 0;
        foreach (string file in Directory.GetFiles(bookCollectionPath))
		{
            if (Path.GetExtension(file) != ".pdf") continue;

            GameObject b = Instantiate(bookPrefab, transform);
            b.GetComponent<Book>().Init($"{Directory.GetCurrentDirectory()}/{file}");
            b.transform.position = new Vector3(i * 2.6f, 0, 0);
            b.transform.Rotate(0, 180, 90);

            //we may or may not want to do this
            b.GetComponent<Book>().Open();

            i++;
        }
    }

}
*//**/