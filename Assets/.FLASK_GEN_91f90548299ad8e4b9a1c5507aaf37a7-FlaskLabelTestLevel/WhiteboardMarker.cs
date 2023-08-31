#define FLASK_ONLY
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//This very heavily derives from this youtube video with modifications made to work with how our objects are layed out
//https://www.youtube.com/watch?v=sHE5ubsP-E8
public class WhiteboardMarker : MonoBehaviour
{
    private bool isRunningUnity = false;
    public Transform _tip;
    public Transform _tipPoint;
    public int _penSize = 5;
    private Renderer _renderer;
    public Renderer _handleRenderer;
    public Color[] _colours;
    public Color _colour;
    public float _tipHeight;
    public LineRenderer line;
    private RaycastHit _touch;
    private Whiteboard _whiteboard;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;
    private void Draw()
    {
        _renderer.material.color = _colour;
        if (Physics.Raycast(_tipPoint.position, transform.up, out _touch, _tipHeight))
        {
            _colours = Enumerable.Repeat(_renderer.material.color, _penSize * _penSize).ToArray();
            if (_touch.transform.name == "WhiteboardDrawable")
            {
                if (isRunningUnity)
                {
                    Debug.Log("The Hit had the tag of " + _touch.transform.name);
                    Debug.Log("Hit " + _touch.ToString());
                }
                else
                {
                    MelonLoader.MelonLogger.Msg(System.ConsoleColor.Green, "The Hit had the tag of " + _touch.transform.name);
                    MelonLoader.MelonLogger.Msg(System.ConsoleColor.Green, "Hit " + _touch.ToString());
                }

                if (_whiteboard == null)
                {
                    _whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);
                var x = (int)(_touchPos.x * _whiteboard.getTexSize().x - (_penSize / 2));
                var y = (int)(_touchPos.y * _whiteboard.getTexSize().y - (_penSize / 2));
                if (y < 0 || y > _whiteboard.tex.height || x < 0 || x > _whiteboard.tex.width)
                    return;
                if (isRunningUnity)
                    Debug.Log("The pen hit (" + x + "," + y + ")");
                else
                    MelonLoader.MelonLogger.Msg(System.ConsoleColor.Green, "The pen hit (" + x + "," + y + ")");
                if (_touchedLastFrame)
                {
                    _whiteboard.tex.SetPixels(x, y, _penSize, _penSize, _colours);
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _whiteboard.tex.SetPixels(lerpX, lerpY, _penSize, _penSize, _colours);
                    }

                    transform.rotation = _lastTouchRot;
                    _whiteboard.tex.Apply();
                }

                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
    }

    void Start()
    {
        //_colour = GetComponentsInChildren<ColourStore>().FirstOrDefault(childRenderer=>childRenderer.CompareTag("PenColour")).penColour;
        _renderer = _tip.GetComponent<Renderer>();
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Draw();
    }
}