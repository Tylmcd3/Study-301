using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Elixir]
/* 
 * This script is a mess at the moment, it needs a good refactor by either 
 *  - Being split into its own classes for the drawing guide and driving the drawing pad or 
 *  - Just break it up into tidier functions. 
 */
public class DrawingTablet : MonoBehaviour
{
    public GameObject DrawingDriver;
    private DrawingTextureManager _drawingDriver;
    public GameObject Display;
    public Renderer _displayRen;
    public GameObject DrawingPad;
    public Renderer _drawingPadRen;
    public Material TransparentMat;
    Material DrawingGuideMat;
    public Texture2D DrawingGuideTex;
    public Rect DrawingPadView;
    public Vector2 DrawingPadViewLastPos;
    public Rect ScreenView;
    public float DrawingPadScale;
    private Vector2 TexSize;
    private Rect lastGuideDraw;
    public KeyCode[] debugMoveKeys;
    public float debugMoveDist;
    public Vector2 getTexSize()
    {
        return _drawingDriver.textureSize;
    }

    public void Draw(int x, int y, int colourSize, Vector2 _lastTouchPos, Color _colour)
    {
        _drawingDriver.Draw(x, y, colourSize, _lastTouchPos, _colour);
    }
    public void Erase(int x, int y, int sizeX, int sizeY, Vector2 _lastTouchPos)
    {
        _drawingDriver.Erase(x, y, sizeX, sizeX, _lastTouchPos);
    }
    private Rect CalculateBoundingBox(Rect rectA, Rect rectB)
    {
        int minX = (int)Mathf.Min(rectA.xMin, rectB.xMin);
        int minY = (int)Mathf.Min(rectA.yMin, rectB.yMin);
        int maxX = (int)Mathf.Max(rectA.xMax, rectB.xMax);
        int maxY = (int)Mathf.Max(rectA.yMax, rectB.yMax);

        int width = maxX - minX;
        int height = maxY - minY;

        return new Rect(minX, minY, width, height);
    }
    void updateDrawingGuide()
    {
        Rect curr = new Rect((int)(TexSize.x * DrawingPadView.x), (int)(TexSize.y * DrawingPadView.y), (int)(TexSize.x * DrawingPadView.width), (int)(TexSize.y * DrawingPadView.height));

        Rect change = CalculateBoundingBox(curr, lastGuideDraw);

        Color _color = Color.blue;
        _color.a = .6f;

        var _colours = Enumerable.Repeat(_color, (int)(curr.width * curr.height)).ToArray();

        DrawingGuideTex.SetPixels((int)curr.x, (int)curr.y, (int)curr.width, (int)curr.height, _colours);
        DrawingGuideTex.Apply();
        lastGuideDraw = curr;
    }

    private Material SetupDrawingGuide()
    {
        DrawingGuideTex = new Texture2D((int)TexSize.x, (int)TexSize.y);
        var _colours = Enumerable.Repeat(new Color(0, 0, 0, 0), (int)(TexSize.x * TexSize.y)).ToArray();
        Material _displayOverlay = new Material(TransparentMat);

        DrawingGuideTex.SetPixels(_colours);
        DrawingGuideTex.Apply();
        _displayOverlay.SetTexture("_BaseMap", DrawingGuideTex);
        return _displayOverlay;
    }
    void Update()
    {
        //MelonLoader.MelonLogger.Msg(System.ConsoleColor.Green, _drawingPadRen.material.name);   
    }

    private void updateDrawingView()
    {
        _drawingPadRen.material.mainTextureOffset = new Vector2(DrawingPadView.x, DrawingPadView.y);
        _drawingPadRen.material.mainTextureScale = new Vector2(DrawingPadView.width, DrawingPadView.height);

        DrawingPadViewLastPos = DrawingPadView.position;
    }

    void Start()
    {
        _drawingDriver = DrawingDriver.GetComponent<DrawingTextureManager>();
        _displayRen = Display.GetComponent<Renderer>();
        _drawingPadRen = DrawingPad.GetComponent<Renderer>();


        float width = (DrawingPad.transform.localScale.x / DrawingPadScale) / Display.transform.localScale.x;
        float height = (DrawingPad.transform.localScale.z / DrawingPadScale) / Display.transform.localScale.z;
        float x = 0.5f - (width * 0.5f);
        float y = 0.5f - (height * 0.5f);

        DrawingPadView = new Rect(x, y, width, height);
        TexSize = _drawingDriver.textureSize;

        DrawingGuideMat = SetupDrawingGuide();

        lastGuideDraw = new Rect((int)(TexSize.x * DrawingPadView.x), (int)(TexSize.y * DrawingPadView.y), (int)(TexSize.x * DrawingPadView.width), (int)(TexSize.y * DrawingPadView.height));


        _displayRen.materials = new Material[] { _drawingDriver.GetSharedMaterial(), DrawingGuideMat };
        updateDrawingGuide();
        MelonLoader.MelonLogger.Msg(System.ConsoleColor.Green, "Before getting Material");
        var m = _drawingDriver.GetSharedMaterial();
        MelonLoader.MelonLogger.Msg(System.ConsoleColor.Green, "Got Material");

        m.mainTextureOffset = new Vector2(DrawingPadView.x, DrawingPadView.y);
        m.mainTextureScale = new Vector2(DrawingPadView.width, DrawingPadView.height);
        _drawingPadRen.material = m;
        

        _displayRen.material.mainTextureOffset = new Vector2(ScreenView.x, ScreenView.y);
        _displayRen.material.mainTextureScale = new Vector2(ScreenView.width, ScreenView.height);

    }


}
