using System.Linq;
using UnityEngine;
[Elixir]
public class Whiteboard : MonoBehaviour
{
    public GameObject DrawingDriver;
    private DrawingTextureManager _drawingDriver;

    public Vector2 getTexSize()
    {
        return _drawingDriver.whiteboardSize;
    }

    public void Draw(int x, int y, int colourSize, Vector2 _lastTouchPos, Color _colour)
    {
        _drawingDriver.Draw(x, y, colourSize, _lastTouchPos, _colour);
    }
    public void Erase(int x, int y, int sizeX, int sizeY, Vector2 _lastTouchPos)
    {
        _drawingDriver.Erase(x, y, sizeX, sizeX, _lastTouchPos);
    }

    void Start()
    {
        _drawingDriver = DrawingDriver.GetComponent<DrawingTextureManager>();
        var r = GetComponent<Renderer>();
        var m = _drawingDriver.CreateSharedMaterial();
        r.material = m;
    }

}
