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
    [HideInInspector] public MeshRenderer _displayRen;
    public GameObject DrawingPad;
    [HideInInspector] public MeshRenderer _drawingPadRen;

    public Material TransparentMat;

    Material DrawingGuideMat;
    public Texture2D DrawingGuideTex;

    public Rect DrawingPadView;
    public Vector2 DrawingPadViewLastPos;
    public Rect ScreenView;
    public float DrawingPadScale;
    private Vector2 TexSize;

    private RectInt lastGuideDraw;
    public KeyCode[] debugMoveKeys;
    public float debugMoveDist;
    public Vector2 getTexSize()
    {
        return new Vector2(_drawingDriver._tex.width, _drawingDriver._tex.height);
    }

    public void Draw(int x, int y, int colourSize, Vector2 _lastTouchPos, Color _colour)
    {
        _drawingDriver.Draw(x, y, colourSize, _lastTouchPos, _colour);
    }
    public void Erase(int x, int y, int sizeX, int sizeY, Vector2 _lastTouchPos)
    {
        _drawingDriver.Erase(x, y, sizeX, sizeX, _lastTouchPos);
    }
    private RectInt CalculateBoundingBox(RectInt rectA, RectInt rectB)
    {
        int minX = Mathf.Min(rectA.xMin, rectB.xMin);
        int minY = Mathf.Min(rectA.yMin, rectB.yMin);
        int maxX = Mathf.Max(rectA.xMax, rectB.xMax);
        int maxY = Mathf.Max(rectA.yMax, rectB.yMax);

        int width = maxX - minX;
        int height = maxY - minY;

        return new RectInt(minX, minY, width, height);
    }
    /*private Color[] ReplaceCertainPixels(Color[] colors, RectInt BoundingBox, RectInt guide)
    {
        Color[] _base = Enumerable.Repeat(new Color(0, 0, 0, 0), (int)(BoundingBox.width * BoundingBox.height)).ToArray();

        for(int i = BoundingBox.yMin; i < BoundingBox.yMax; i++)
        {
            if (i < guide.yMin || i > guide.yMax) continue;
            for (int j = BoundingBox.xMin; j < BoundingBox.xMax; j++)
            {
                if (j < guide.xMin || j > guide.xMax) continue;

                int YBase = i - BoundingBox.yMin;
                int YColors = i - guide.yMin;
                int xBase = j - BoundingBox.xMin;
                int xColors = j - guide.xMin;


                Debug.Log($"I = {i}, j = {j}");
                Debug.Log($"xBase = {xBase}, yBase = {YBase}")
                _base[xBase + (BoundingBox.width * YBase)] = colors[YColors + (BoundingBox.width * xColors)];

            }
        }


        return _base;

    }*/
    void updateDrawingGuide()
    {
        RectInt curr = new RectInt((int)(TexSize.x * DrawingPadView.x), (int)(TexSize.y * DrawingPadView.y), (int)(TexSize.x * DrawingPadView.width), (int)(TexSize.y * DrawingPadView.height));

        Debug.Log(curr);
        RectInt change = CalculateBoundingBox(curr, lastGuideDraw);

        Color _color = Color.blue;
        _color.a = .6f;

        var _colours = Enumerable.Repeat(_color, curr.width * curr.height).ToArray();

        //var _newCols = ReplaceCertainPixels(_colours, change, curr);

        //DrawingGuideTex.SetPixels(change.x, change.y, change.width, change.height, _newCols);
        DrawingGuideTex.SetPixels(curr.x, curr.y, curr.width, curr.height, _colours);
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
    private void Update()
    {
        /*if (DrawingPadView.position != DrawingPadViewLastPos)
        {
            updateDrawingView();
            updateDrawingGuide();
        }
        if (Input.GetKeyDown(debugMoveKeys[0]))
        {
            DrawingPadView.y += debugMoveDist;

            if (DrawingPadView.y >= 1 - DrawingPadView.height) DrawingPadView.y = 1 - DrawingPadView.height;
        }
        if (Input.GetKeyDown(debugMoveKeys[1]))
        {
            DrawingPadView.y -= debugMoveDist;
            if (DrawingPadView.y < 0) DrawingPadView.y = 0;
        }
        if (Input.GetKeyDown(debugMoveKeys[2]))
        {
            DrawingPadView.x -= debugMoveDist;
            if (DrawingPadView.x < 0) DrawingPadView.x = 0;
        }
        if (Input.GetKeyDown(debugMoveKeys[3]))
        {
            DrawingPadView.x += debugMoveDist;
            if (DrawingPadView.x >= 1 - DrawingPadView.width) DrawingPadView.x = 1 - DrawingPadView.width;
        }*/
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
        _displayRen = Display.GetComponent<MeshRenderer>();
        _drawingPadRen = DrawingPad.GetComponent<MeshRenderer>();

        float width =  (DrawingPad.transform.localScale.x/ DrawingPadScale) / Display.transform.localScale.x;
        float height = (DrawingPad.transform.localScale.z / DrawingPadScale) / Display.transform.localScale.z;
        float x = 0.5f - (width * 0.5f);
        float y = 0.5f - (height * 0.5f);
        DrawingPadView = new Rect(x, y, width, height);
        TexSize = _drawingDriver.textureSize;
        DrawingGuideMat = SetupDrawingGuide();

        lastGuideDraw = new RectInt((int)(TexSize.x * DrawingPadView.x), (int)(TexSize.y * DrawingPadView.y), (int)(TexSize.x * DrawingPadView.width), (int)(TexSize.y * DrawingPadView.height));


        _displayRen.materials = new Material[] { _drawingDriver.GetMat(), DrawingGuideMat };
        updateDrawingGuide();

        _drawingPadRen.material = _drawingDriver.GetMat(TransparentMat);

        _displayRen.material.mainTextureOffset = new Vector2(ScreenView.x, ScreenView.y);
        _displayRen.material.mainTextureScale = new Vector2(ScreenView.width, ScreenView.height);

        _drawingPadRen.material.color = new Color(1, 1, 1, 0.45f);
        _drawingPadRen.material.mainTextureOffset = new Vector2(DrawingPadView.x, DrawingPadView.y);
        _drawingPadRen.material.mainTextureScale = new Vector2(DrawingPadView.width, DrawingPadView.height);

    }

    
}
