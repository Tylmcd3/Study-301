using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Elixir]
//This is also derived from the video
//https://www.youtube.com/watch?v=sHE5ubsP-E8

//This version is made to erase using the same method as before 
public class WhiteboardEraser : MonoBehaviour
{
    // Start is called before the first frame update
    public int _penSize = 5;

    public float CornerDeviation;
    private Renderer _renderer;
    public Color[] _colours;
    public bool TestColourToggle;
    public Color TestColour;
    public float _tipHeight;
    private Vector3 _size;

    private RaycastHit _touchTop;
    private RaycastHit _touchBottom;
    private DrawingTablet _tablet;
    private Whiteboard _whiteboard;
    private Vector2 _touchPosTop;
    private Vector2 _touchPosBottom;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;
    public Transform WhiteboardObj;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _size = _renderer.bounds.size;
        
    }


    void Update()
    {
        Draw();
    }

    private void Draw()
    {
        Vector3 TopLeft = new Vector3(transform.position.x - _size.x / 2, transform.position.y + (_size.y / 2), transform.position.z + _size.z / 2);
        Vector3 BottomRight = new Vector3(transform.position.x - _size.x / 2, transform.position.y - (_size.y / 2), transform.position.z - _size.z / 2);
        
        if (Physics.Raycast(TopLeft, transform.up, out _touchTop, _tipHeight))
        {
            Physics.Raycast(BottomRight, transform.up, out _touchBottom, _tipHeight);
            if (_touchTop.transform.CompareTag("Whiteboard"))
            {

                if (_whiteboard == null)
                {
                    _whiteboard = _touchTop.transform.GetComponent<Whiteboard>();
                }

                Vector2 TexSize = _whiteboard.getTexSize();

                _touchPosTop = new Vector2(_touchTop.textureCoord.x, _touchTop.textureCoord.y);
                _touchPosBottom = new Vector2(_touchBottom.textureCoord.x, _touchBottom.textureCoord.y);

                var x1 = (int)(_touchPosTop.x * TexSize.x - (_penSize / 2));
                var y1 = (int)(_touchPosTop.y * TexSize.y - (_penSize / 2));

                var x2 = (int)(_touchPosBottom.x * TexSize.x - (_penSize / 2));
                var y2 = (int)(_touchPosBottom.y * TexSize.y - (_penSize / 2));

                
                if (x1 < 0) x1 = 0;
                else if(x1 > TexSize.x) x1 = (int)TexSize.x;

                if (y1 < 0) y1 = 0;
                else if (y1 > TexSize.y) y1 = (int)TexSize.y;

                if (x2 < 0) x2 = 0;
                else if (x2 > TexSize.x) x2 = (int)TexSize.x;

                if (y2 < 0) y2 = 0;
                else if (y2 > TexSize.y) y2 = (int)TexSize.y;

                if (x1 < x2) { var temp = x2; x2 = x1; x1 = temp; }
                if (y1 < y2) { var temp = y2; y2 = y1; y1 = temp; }

                if (_touchedLastFrame)
                {
                    var eraseSize = new Vector2(x1-x2, y1-y2);
                    _whiteboard.Erase(x2,y2, Math.Abs((int)eraseSize.x), Math.Abs((int)eraseSize.y), _lastTouchPos);
                    transform.rotation = _lastTouchRot;
                }
                _lastTouchPos = new Vector2(x2, y2);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;

                return;
            }
        }

        _whiteboard = null;
        _touchedLastFrame = false;
    }
}
