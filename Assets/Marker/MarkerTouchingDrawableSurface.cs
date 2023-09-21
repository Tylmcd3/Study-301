using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[Elixir]
public class MarkerTouchingDrawableSurface : MonoBehaviour
{
    public Transform _tip;
    public Transform _wand;
    public int _penSize = 5;

    public Renderer _handleRenderer;
    public Color[] _colours;
    public Color _colour;
    public float _tipHeight;

    private RaycastHit _touch;
    private DrawingTablet _Tablet;
    private Whiteboard _Whiteboard;
    private Vector2 _touchPos;
    private bool _touchedLastFrame;
    private Vector2 _lastTouchPos;
    private Quaternion _lastTouchRot;

    public float beamLength = 2.0f;
    public float markerMaxSize = 1.0f;
    public float markerMinSize = 0.2f;
    public int markerWandEndScale = 5;
    public int numberOfSegments = 16;
    public Shader shader;

    private MeshRenderer WandRenderer;
    private Renderer TipRenderer;

    private Mesh mesh;

    public float PenBaseSize = 0.5f;
    public float PenTipSize = 1f;

    public Material Transparent;
    public Material Opaque;

    public GameObject DrawingDriver;
    private DrawingTextureManager _drawingDriver;

    // Start is called before the first frame update
    void Start()
    {
        Opaque = new Material(shader);

        TipRenderer = _tip.GetComponent<MeshRenderer>();
        WandRenderer = _wand.GetComponent<MeshRenderer>();
        _drawingDriver = DrawingDriver.GetComponent<DrawingTextureManager>();
        GeneratePenTip();
        GenerateDepthWand();
    }
    void Update(){
        Draw();
    }
    void SetColour()
    {
        TipRenderer.material.color = _colour;
        WandRenderer.material.color = _colour;
    }
    void GeneratePenTip()
    {
        Mesh mesh = new Mesh();
        _tip.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[3 * (numberOfSegments + 1)];
        int[] triangles = new int[3 * 5 * (numberOfSegments + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];

        // Generate vertices
        for (int i = 0; i < numberOfSegments; i++)
        {
            float angle = 2 * 3.14159265f * i / numberOfSegments;
            float x1 = (markerMaxSize / 200) * Mathf.Cos(angle);
            float z1 = (markerMaxSize / 200) * Mathf.Sin(angle);

            float x2 = (markerMinSize / 200) * Mathf.Cos(angle);
            float z2 = (markerMinSize / 200) * Mathf.Sin(angle);

            vertices[i] = new Vector3(x1, 0, z1);
            vertices[i + numberOfSegments] = new Vector3(x1, PenBaseSize, z1);
            vertices[i + (numberOfSegments * 2)] = new Vector3(x2, PenTipSize, z2);
        }

        vertices[numberOfSegments * 3] = new Vector3(0, PenTipSize, 0);

        // Generate triangles
        int vertIndex = 0;
        int triIndex = 0;
        for (int i = 0; i < numberOfSegments; i++)
        {
            //Base Ring
            triangles[triIndex] = i;
            triangles[triIndex + 1] = numberOfSegments + (i + 1) % numberOfSegments;
            triangles[triIndex + 2] = (i + 1) % numberOfSegments;

            triangles[triIndex + 3] = i;
            triangles[triIndex + 4] = numberOfSegments + i;
            triangles[triIndex + 5] = numberOfSegments + ((i + 1) % numberOfSegments);
            
            //Ring to Tip
            triangles[triIndex + 6] = numberOfSegments + i;
            triangles[triIndex + 7] = (numberOfSegments * 2) + (i + 1) % numberOfSegments;
            triangles[triIndex + 8] = numberOfSegments + (i + 1) % numberOfSegments;

            triangles[triIndex + 9] = numberOfSegments + i;
            triangles[triIndex + 10] = (numberOfSegments * 2) + i;
            triangles[triIndex + 11] = (numberOfSegments * 2) + ((i + 1) % numberOfSegments);

            //Tip
            triangles[triIndex + 12] = (numberOfSegments * 2) + i;
            triangles[triIndex + 13] = numberOfSegments * 3;
            triangles[triIndex + 14] = (numberOfSegments * 2) + ((i + 1) % numberOfSegments);

            vertIndex++;
            triIndex += 15;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Create a new material and apply it to the mesh renderer
        Material material = new Material(shader);
        material.color = _colour;
        TipRenderer.material = material;

        TipRenderer.GetComponent<MeshCollider>().sharedMesh = mesh;

    }
    void GenerateDepthWand()
    {
        _wand.localPosition = _tip.localPosition + new Vector3(0, PenTipSize, 0);
        Mesh mesh = new Mesh();
        _wand.gameObject.GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[2 * (numberOfSegments + 1)];
        int[] triangles = new int[3 * 3 * numberOfSegments];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];

        // Generate vertices
        for (int i = 0; i < numberOfSegments; i++)
        {
            float angle = 2 * 3.14159265f * i / numberOfSegments;
            float x1 = (markerMinSize/ 200) * Mathf.Cos(angle);
            float z1 = (markerMinSize / 200) * Mathf.Sin(angle);
            
            float x2 = ((markerMinSize / 200)/markerWandEndScale) * Mathf.Cos(angle);
            float z2 = ((markerMinSize / 200)/markerWandEndScale) * Mathf.Sin(angle);
            vertices[i] = new Vector3(x1, 0, z1);
            vertices[i + numberOfSegments] = new Vector3(x2, beamLength, z2);
        }

        vertices[numberOfSegments * 2] = new Vector3(0, beamLength, 0);

        // Generate triangles
        int vertIndex = 0;
        int triIndex = 0;
        for (int i = 0; i < numberOfSegments; i++)
        {
            //Base Ring
            triangles[triIndex] = i;
            triangles[triIndex + 1] = numberOfSegments + (i + 1) % numberOfSegments;
            triangles[triIndex + 2] = (i + 1) % numberOfSegments;

            triangles[triIndex + 3] = i;
            triangles[triIndex + 4] = numberOfSegments + i;
            triangles[triIndex + 5] = numberOfSegments + ((i + 1) % numberOfSegments);

            //Tip
            triangles[triIndex + 6] = numberOfSegments + i;
            triangles[triIndex + 7] = numberOfSegments * 2;
            triangles[triIndex + 8] = numberOfSegments + ((i + 1) % numberOfSegments);


            vertIndex++;
            triIndex += 9;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Create a new material and apply it to the mesh renderer
        Material mat = new Material(Transparent);
        mat.color = _colour;
        WandRenderer.material = mat;
    }

    private void Draw()
    {
        if (Physics.Raycast(_wand.position, transform.up, out _touch, beamLength/2)) //beamLength is 1/2 because The Mesh size is halved
        {

            float normalsedDistance = 1 - ((_touch.distance * 2) / beamLength);

            normalsedDistance = (normalsedDistance < 0) ? 0 : normalsedDistance;
            normalsedDistance = (normalsedDistance > 1) ? 1 : normalsedDistance;
            int colourSize = Mathf.RoundToInt(Mathf.Lerp(markerMinSize, markerMaxSize, normalsedDistance));
            if (_touch.collider.transform.name == "TabletDrawable")
            {
                if (_Tablet == null)
                {
                    _Tablet = _touch.transform.GetComponentInParent<DrawingTablet>();
                }
                Vector2 texSize = _drawingDriver.textureSize;

                Vector3 worldSpaceHitPoint = _touch.point;
                Vector3 localSpaceHitPoint = _touch.collider.transform.worldToLocalMatrix.MultiplyPoint(worldSpaceHitPoint);

                //Gross maths that is getting the hit point, converting it to the colliders local space, making that a 0-1 value and inverting it.
                //The +5 and /10 come from the range I was getting from local Space hit was -5 to 5, so +5 offsets it to 0-10 and divide by 10 to normalise.
                _touchPos = new Vector2(1-((localSpaceHitPoint.x + 5)/10), 1 - ((localSpaceHitPoint.z + 5) / 10));

                    
                Transform Hit = _touch.transform;
                Renderer Ren = Hit.GetComponent<Renderer>();

                Vector2 offset = new Vector2(texSize.x * _Tablet.DrawingPadView.x, texSize.y * _Tablet.DrawingPadView.y);

                int x = (int)(offset.x + (_touchPos.x * (texSize.x * _Tablet.DrawingPadView.width)) - (colourSize / 2));
                int y = (int)(offset.y + (_touchPos.y * (texSize.y * _Tablet.DrawingPadView.height)) - (colourSize / 2));

                if (y < 0 || y > texSize.y || x < 0 || x > texSize.x) return;

                if (_touchedLastFrame)
                {
                    _drawingDriver.Draw(x, y, colourSize, _lastTouchPos, TipRenderer.material.color);
                    if (normalsedDistance > 0.98f)
                        transform.rotation = _lastTouchRot;
                }
                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;

                return;
            }
            else if (_touch.transform.name == "WhiteboardDrawable")
            {
                //Debug.Log(normalsedDistance);

                if (_Whiteboard == null)
                {
                    _Whiteboard = _touch.transform.GetComponent<Whiteboard>();
                }
                Vector2 texSize = _drawingDriver.textureSize;

                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);

                int x = (int)(_touchPos.x * texSize.x - (colourSize / 2));
                int y = (int)(_touchPos.y * texSize.y - (colourSize / 2));
                if (y < 0 || y > texSize.y || x < 0 || x > texSize.x) return;

                if (_touchedLastFrame)
                {
                    _drawingDriver.Draw(x, y, colourSize, _lastTouchPos, TipRenderer.material.color);
                    if(normalsedDistance > 0.98f)
                        transform.rotation = _lastTouchRot;
                }
                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;

                return;
            }
           
        }


        _Tablet = null;
        _Whiteboard = null;
        _touchedLastFrame = false;
    }
}
