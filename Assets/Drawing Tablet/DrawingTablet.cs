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
    private Vector2Int TexSize;
    private Rect lastGuideDraw;
    public KeyCode[] debugMoveKeys;
    public float debugMoveDist;
    public Vector2 getTexSize()
    {
        return _drawingDriver.whiteboardSize;
    }
    public void updateDrawingGuide()
    {
        Texture2D tempTex = DrawingGuideTex;
        DrawingPadView.x = (DrawingPadView.x < 0) ? 0 : (DrawingPadView.x > 1 - DrawingPadView.width) ? 1 - DrawingPadView.width : DrawingPadView.x;
        DrawingPadView.y = (DrawingPadView.y < 0) ? 0 : (DrawingPadView.y > 1 - DrawingPadView.height) ? 1 - DrawingPadView.height : DrawingPadView.y;
        Rect curr = new Rect((int)(TexSize.x * DrawingPadView.x), (int)(TexSize.y * DrawingPadView.y), (int)(TexSize.x * DrawingPadView.width), (int)(TexSize.y * DrawingPadView.height));

        Color _color = Color.blue;
        _color.a = .6f;

        var _colours = Enumerable.Repeat(_color, (int)(curr.width * curr.height)).ToArray();
        var _clear = Enumerable.Repeat(Color.clear, (int)(lastGuideDraw.width * lastGuideDraw.height)).ToArray();

        // Im sure this is super intensive
        DrawingGuideTex.SetPixels((int)lastGuideDraw.x, (int)lastGuideDraw.y, (int)lastGuideDraw.width, (int)lastGuideDraw.height, _clear);
        DrawingGuideTex.SetPixels((int)curr.x, (int)curr.y, (int)curr.width, (int)curr.height, _colours);
        DrawingGuideTex.Apply();
        lastGuideDraw = curr;
    }

    private Material SetupDrawingGuide()
    {
        DrawingGuideTex = new Texture2D(TexSize.x, TexSize.y);
        var _colours = Enumerable.Repeat(new Color(0, 0, 0, 0), (TexSize.x * TexSize.y)).ToArray();
        Material _displayOverlay = new Material(TransparentMat);

        DrawingGuideTex.SetPixels(_colours);
        DrawingGuideTex.Apply();
        _displayOverlay.SetTexture("_BaseMap", DrawingGuideTex);
        return _displayOverlay;
    }
    public void updateDrawingView()
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
        TexSize = _drawingDriver.whiteboardSize;

        DrawingGuideMat = SetupDrawingGuide();

        lastGuideDraw = new Rect((int)(TexSize.x * DrawingPadView.x), (int)(TexSize.y * DrawingPadView.y), (int)(TexSize.x * DrawingPadView.width), (int)(TexSize.y * DrawingPadView.height));


        _displayRen.materials = new Material[] { _drawingDriver.CreateSharedMaterial(), DrawingGuideMat };
        updateDrawingGuide();
        var m = _drawingDriver.CreateSharedMaterial();

        m.mainTextureOffset = new Vector2(DrawingPadView.x, DrawingPadView.y);
        m.mainTextureScale = new Vector2(DrawingPadView.width, DrawingPadView.height);
        _drawingPadRen.material = m;

    }


}
