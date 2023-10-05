/*using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour
{

    static Color[] colors = new Color[] {Color.red, Color.blue, Color.green, Color.cyan, Color.magenta, Color.yellow};
    string path = "F:/.Downloads/jov-11-5-8.pdf";
    string title;

    public void Init(string p)
    {
        path = p;        
        title = Path.GetFileName(path);
        title = title.Substring(0, title.Length - 4);
        transform.GetChild(1).GetComponent<TextMeshPro>().text = title;
        SetSeededColour();
    }

    void SetSeededColour()
	{
        int seed = 0;
        foreach (char c in title)
		{
            seed += (int)c;
		}
        seed %= colors.Length;

        Texture2D tex = new Texture2D(2, 1);
        tex.SetPixel(0, 0, colors[seed]);
        tex.SetPixel(1, 0, Color.grey);
        tex.Apply();
        GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", tex);
    }

    public void Open()
	{
        GetComponentInChildren<PdfNavigator>().Open(path);
    }

}
*/