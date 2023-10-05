using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Elixir]
public class MarkerTouchingDrawableSurface : MonoBehaviour
{
    public Transform Tip;
    public Transform Wand;
    public int PenSize = 5;

    public GameObject DrawingDriver;
    public Renderer HandleRenderer;
    public Color[] Colours;
    public Color Colour;
    public float TipHeight;

    private RaycastHit _touch;
    private DrawingTablet _Tablet;
    private Whiteboard _Whiteboard;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;

    private MeshRenderer _wandRenderer;
    private Renderer _tipRenderer;
    private DrawingTextureManager _drawingDriver;
    private MarkerBuilder _markerBuilder;

    // Start is called before the first frame update
    void Start()
    {
        _wandRenderer = Wand.GetComponent<MeshRenderer>();
        _tipRenderer = Tip.GetComponent<MeshRenderer>();
        _drawingDriver = DrawingDriver.GetComponent<DrawingTextureManager>();
        _markerBuilder = GetComponent<MarkerBuilder>();
        _markerBuilder.GeneratePenTip(Tip, Colour);
        _markerBuilder.GenerateDepthWand(Tip, Wand, Colour);
    }
    void Update(){
        Draw(); 
    }
    void SetColour(Color colour)
    {
        Colour = colour;
        _tipRenderer.material.color = colour;
        _wandRenderer.material.color = colour;
    }
    private void Draw()
    {
        if (Physics.Raycast(Wand.position, transform.up, out _touch, _markerBuilder.beamLength / 2)) //beamLength is 1/2 because The Mesh size is halved
        {
            Vector2 texSize = _drawingDriver.whiteboardSize;
            if (_touch.collider.transform.name.Contains("Drawable"))
            {
                float normalsedDistance = 1 - ((_touch.distance * 2) / _markerBuilder.beamLength);
                normalsedDistance = Mathf.Clamp01(normalsedDistance);
                int colourSize = Mathf.RoundToInt(Mathf.Lerp(_markerBuilder.markerMinSize, _markerBuilder.markerMaxSize, normalsedDistance));
                Vector2Int coords = getDrawingCoords(colourSize);

                if (coords.y < 0 || coords.y > texSize.y || coords.x < 0 || coords.x > texSize.x) return;

                if (_touchedLastFrame)
                {
                    _drawingDriver.Draw(coords.x, coords.y, colourSize, _lastTouchPos, _tipRenderer.material.color);
                    if (normalsedDistance > 0.98f)
                        transform.rotation = _lastTouchRot;
                }
                _lastTouchPos = new Vector2Int(coords.x, coords.y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;

                return;

            }
            else if (_touch.collider.transform.name == "DisplayScreen")
            {
                int colourSize = 1;

                if (_Tablet == null)
                {
                    _Tablet = _touch.transform.GetComponentInParent<DrawingTablet>();
                }

                Vector3 localSpaceHitPoint = _touch.collider.transform.worldToLocalMatrix.MultiplyPoint(_touch.point);

                //Gross maths that is getting the hit point, converting it to the colliders local space, making that a 0-1 value and inverting it.
                //The +5 and /10 come from the range I was getting from local Space hit was -5 to 5, so +5 offsets it to 0-10 and divide by 10 to normalise.
                _touchPos = new Vector2(1 - ((localSpaceHitPoint.x + 5) / 10), 1 - ((localSpaceHitPoint.z + 5) / 10));
                if (_touchedLastFrame)
                {
                    Vector2 diff  = _touchPos - _lastTouchPos;
                    if(diff.x != 0 && diff.y != 0)
                    {
                        _Tablet.DrawingPadView.x = _Tablet.DrawingPadView.x + diff.x;
                        _Tablet.DrawingPadView.y = _Tablet.DrawingPadView.y + diff.y;

                        _Tablet.updateDrawingGuide();
                        _Tablet.updateDrawingView();
                    }
                }

                _lastTouchPos = _touchPos;
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;

                return;
            }
            
        }
        _Tablet = null;
        _Whiteboard = null;
        _touchedLastFrame = false;

    }

    Vector2Int getDrawingCoords(int colourSize)
    {
        Vector2Int coords = new Vector2Int(-1, -1);
        Vector2 texSize = _drawingDriver.whiteboardSize;

        if (_touch.collider.transform.name == "DrawableTablet")
        {
            if (_Tablet == null)
            {
                _Tablet = _touch.transform.GetComponentInParent<DrawingTablet>();
            }

            Vector3 localSpaceHitPoint = _touch.collider.transform.worldToLocalMatrix.MultiplyPoint(_touch.point);

            //Gross maths that is getting the hit point, converting it to the colliders local space, making that a 0-1 value and inverting it.
            //The +5 and /10 come from the range I was getting from local Space hit was -5 to 5, so +5 offsets it to 0-10 and divide by 10 to normalise.
            _touchPos = new Vector2(1 - ((localSpaceHitPoint.x + 5) / 10), 1 - ((localSpaceHitPoint.z + 5) / 10));

            Vector2 offset = new Vector2(texSize.x * _Tablet.DrawingPadView.x, texSize.y * _Tablet.DrawingPadView.y);

            coords.x = (int)(offset.x + (_touchPos.x * (texSize.x * _Tablet.DrawingPadView.width)) - (colourSize / 2));
            coords.y = (int)(offset.y + (_touchPos.y * (texSize.y * _Tablet.DrawingPadView.height)) - (colourSize / 2));
        }
        else if (_touch.transform.name == "DrawableWhiteboard")
        {
            if (_Whiteboard == null)
            {
                _Whiteboard = _touch.transform.GetComponent<Whiteboard>();
            }

            _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

            coords.x = (int)(_touchPos.x * texSize.x - (colourSize / 2));
            coords.y = (int)(_touchPos.y * texSize.y - (colourSize / 2));
        }
        return coords;
    }
}
