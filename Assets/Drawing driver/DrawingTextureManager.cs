using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Elixir]
public class DrawingTextureManager : MonoBehaviour
{
    public bool showTestTex;
    [HideInInspector] public Texture2D _tex;
    public Texture2D testTex;
    public Material mat;

    public Vector2 textureSize = new Vector2(2048, 2048);

    // Start is called before the first frame update
    void Start()
    {
        if(showTestTex)
        {
            _tex = Instantiate(testTex);
        }
        else
        {
            //Setup Texture
            _tex = new Texture2D((int)textureSize.x, (int)textureSize.y);
            var color = Enumerable.Repeat(Color.white, (int)(textureSize.x * textureSize.y)).ToArray();
            _tex.SetPixels(color);
            _tex.Apply();
        }
        textureSize = new Vector2(_tex.width, _tex.height);


    }
    public Material GetMat(Material applyMat = null)
    {
        Material newMat;
        if (applyMat == null)
        {
            newMat = new Material(mat);
        }
        else
        {
            newMat = new Material(applyMat);
        }
        newMat.SetTexture("_BaseMap", _tex);
        return newMat;
    }

    public void Draw(int x, int y, int colourSize, Vector2 _lastTouchPos, Color _colour)
    {
        Color[] _colours = Enumerable.Repeat(_colour, colourSize * colourSize).ToArray();

        _tex.SetPixels(x, y, colourSize, colourSize, _colours);
        for (float f = 0.01f; f < 1.00f; f += 0.01f)
        {
            var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
            var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
            _tex.SetPixels(lerpX, lerpY, colourSize, colourSize, _colours);
        }
        _tex.Apply();

    }
    public void Erase(int x, int y, int sizeX, int sizeY, Vector2 _lastTouchPos)
    {
        var _colours = Enumerable.Repeat(Color.white, (int)(sizeX * sizeY)).ToArray();
        _tex.SetPixels(x, y, sizeX, sizeY, _colours);
        for (float f = 0.01f; f < 1.00f; f += 0.01f)
        {
            var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
            var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
            _tex.SetPixels(lerpX, lerpY, sizeX, sizeY, _colours);
        }
        _tex.Apply();
    }

}
