using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//This very heavily derives from this youtube video with modifications made to work with how our objects are layed out
//https://www.youtube.com/watch?v=sHE5ubsP-E8

public class WhiteboardMarker : MonoBehaviour
{
    [SerializeField]private bool isRunningUnity = false;
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

    

    void Start()
    {
        //_colour = GetComponentsInChildren<ColourStore>().FirstOrDefault(childRenderer=>childRenderer.CompareTag("PenColour")).penColour;
        
    }
    

    
}
