using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


[Elixir]
public class DrawingTextureManager : MonoBehaviour
{
    public enum SlideChange
    {
        left = -1,
        right = 1
    }
    // Public Variables
    public Vector2Int whiteboardSize;
    public Color whiteboardBaseColour = Color.white;
    public Material baseMaterial;
    public string saveDir;
    
    //Private Storage
    private List<Material> _sharedMaterials;
    private List<Texture2D> _whiteboardPages;
    private List<Texture2D> _pdfPages;

    //States
    private bool isWhiteboard = true;
    private Texture2D _activeTexture;
    private int _pdfIndex;
    private int _whiteboardIndex;

    private void Update()
    {
        //Debug keys for testing
        if (Input.GetKeyDown(KeyCode.Keypad1)) { DeleteCurrentSlide(); }
        if (Input.GetKeyDown(KeyCode.Keypad2)) { ExportTextures(); }
        if (Input.GetKeyDown(KeyCode.Keypad3)) { CreateNewSlide(); }
        if (Input.GetKeyDown(KeyCode.Keypad4)) { MoveSlide(SlideChange.left); }
        if (Input.GetKeyDown(KeyCode.Keypad5)) { BlankCurrentSlide(); }
        if (Input.GetKeyDown(KeyCode.Keypad6)) { MoveSlide(SlideChange.right); }
        if (Input.GetKeyDown(KeyCode.Keypad7)) { LoadPDFIntoManager(false); }
        if (Input.GetKeyDown(KeyCode.Keypad8)) { ToggleDisplay(); }
    }
    void Awake()
    {
        _sharedMaterials = new List<Material>();
        _whiteboardPages = new List<Texture2D>();

        CreateNewSlide();
    }

    // Start is called before the first frame update
    void Start()
    {
        //DrawFramer = new STUDYKeyframer(name, nameof(Draw), (object args) => Draw((DrawArgs)args), typeof(DrawArgs));
        //EraseFramer = new STUDYKeyframer(name, nameof(Erase), (object args) => Erase((EraseArgs)args), typeof(EraseArgs));
    }
    public Texture2D GetSharedTexture() { return _activeTexture; }
    public int GetPageIndex() { return (isWhiteboard) ? _whiteboardIndex : _pdfIndex; }
    public Material CreateSharedMaterial()
    {
        Material newMat = new Material(baseMaterial);
        newMat.mainTexture = _activeTexture;
        _sharedMaterials.Add(newMat);

        return newMat;
    }
    private void UpdateSharedTexture()
    {
        _activeTexture = (isWhiteboard) ? _whiteboardPages[_whiteboardIndex] : _pdfPages[_pdfIndex];
        foreach (Material mat in _sharedMaterials)
        {
            mat.SetTexture("_BaseMap", _activeTexture);
        }
    }
    public void CreateNewSlide(int index = -1)
    {
        if (isWhiteboard)
        {
            Texture2D slide = new Texture2D(whiteboardSize.x, whiteboardSize.y);
            Color[] color = Enumerable.Repeat(Color.white, (whiteboardSize.x * whiteboardSize.y)).ToArray();

            slide.SetPixels(color);
            slide.Apply();

            if (index == -1)
            {
                _whiteboardPages.Add(slide);
                _whiteboardIndex = _whiteboardPages.Count - 1;
            }
            else
            {
                _whiteboardPages.Insert(index, slide);
            }

            UpdateSharedTexture();
        }
        else
            Debug.Log("You cannot insert slides into a pdf");
    }
    public void MoveSlide(string dir)
    {
        if(dir == "left")
        {
            MoveSlide(SlideChange.left);
        }
        else
        {
            MoveSlide(SlideChange.right);

        }
    }
    // Loads of ternary operators here, not super elegant but stops duplicate code
    public void MoveSlide(SlideChange dir)
    {
        int newIndex = isWhiteboard ? (_whiteboardIndex + (int)dir) : (_pdfIndex + (int)dir);
        int count = isWhiteboard ? _whiteboardPages.Count : _pdfPages.Count;
        if (newIndex >= 0 && newIndex < count)
        {
            _ = (isWhiteboard) ? (_whiteboardIndex = newIndex) : (_pdfIndex = newIndex);
            UpdateSharedTexture();
        }
    }
    public void BlankCurrentSlide()
    {
        if(isWhiteboard)
            CreateNewSlide( _whiteboardIndex);
        else
            Debug.Log("You cannot blank pdf slides");
    }
    public void DeleteCurrentSlide()
    {
        if(isWhiteboard)
        { 
            _whiteboardPages.RemoveAt(_whiteboardIndex);
            if (_whiteboardIndex > 0)
            {
                _whiteboardIndex--;
                UpdateSharedTexture();
            }
            else
            {
                CreateNewSlide();
            }
        }
        else
            Debug.Log("You cannot Delete pdf slides");

    }
    //TODO: Convert this to PDF export

    public void ExportTextures()
    {
        if (isWhiteboard)
        {
            string savePath = Path.Combine(Application.dataPath, saveDir);

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);
            long time = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            for (int i = 0; i < _whiteboardPages.Count; i++)
            {
                string textureName = time + "-Slide" + i + ".png"; // You can customize the file name.
                string filePath = Path.Combine(savePath, textureName);
                Debug.Log(filePath);
                byte[] bytes = _whiteboardPages[i].EncodeToPNG();

                File.WriteAllBytes(filePath, bytes);
            }
        }
        else
            Debug.Log("PDF Exporting is not yet implemented");
    }

    public void LoadPDFIntoManager(bool showPDF) {
        List<Texture2D> pdf = GetComponent<pdfStorage>().getPDF();

        if (pdf != null)
        {
            _pdfPages = pdf;
            _pdfIndex = 0;
            isWhiteboard = !showPDF;
            UpdateSharedTexture();
        }

    }
    public void ToggleDisplay()
    {
        if(_pdfPages.Count <= 0)
        {
            LoadPDFIntoManager(false);
        }
        isWhiteboard = !isWhiteboard;
        UpdateSharedTexture();
    }


    //STUDYKeyframer DrawFramer;
    //STUDYKeyframer EraseFramer;
    public struct DrawArgs { public int x, y, coloursize; public Vector2 _lastTouchPos; public Color _colour; }
    public struct EraseArgs { public int x, y, sizeX, sizeY; public Vector2 _lastTouchPos; }
    public void Draw(DrawArgs args) { Draw(args.x, args.y, args.coloursize, args._lastTouchPos, args._colour); }
    public void Erase(EraseArgs args) { Erase(args.x, args.y, args.sizeX, args.sizeY, args._lastTouchPos); }
    // These will Draw and erase to the current 
    public void Draw(int x, int y, int colourSize, Vector2 _lastTouchPos, Color _colour)
    {
        //DrawFramer.AddFrame(new DrawArgs { x = x, y = y, coloursize = colourSize, _lastTouchPos = _lastTouchPos, _colour = _colour });

        Color[] _colours = Enumerable.Repeat(_colour, colourSize * colourSize).ToArray();

        _activeTexture.SetPixels(x, y, colourSize, colourSize, _colours);
        for (float f = 0.01f; f < 1.00f; f += 0.01f)
        {
            var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
            var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
            _activeTexture.SetPixels(lerpX, lerpY, colourSize, colourSize, _colours);
        }
        _activeTexture.Apply();

    }
    public void Erase(int x, int y, int sizeX, int sizeY, Vector2 _lastTouchPos)
    {
        //EraseFramer.AddFrame(new EraseArgs { x = x, y = y, sizeX = sizeX, sizeY = sizeY, _lastTouchPos = _lastTouchPos });

        var _colours = Enumerable.Repeat(Color.white, (int)(sizeX * sizeY)).ToArray();
        _activeTexture.SetPixels(x, y, sizeX, sizeY, _colours);
        for (float f = 0.01f; f < 1.00f; f += 0.01f)
        {
            var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
            var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
            _activeTexture.SetPixels(lerpX, lerpY, sizeX, sizeY, _colours);
        }
        _activeTexture.Apply();
    }

}
