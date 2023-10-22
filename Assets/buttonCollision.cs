using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static DrawingTextureManager;

[Elixir]
public class buttonCollision : MonoBehaviour
{
    public bool toggleGroup = false;
    public string document;
    public GameObject DocumentGroup;
    public GameObject manager;
    private ButtonPressTimer timer;

    void OnTriggerEnter(Collider other)
    {
        if (!timer.CheckTime())
        {
            return;
        }
        if (toggleGroup)
        {
            pdfStorage storage = manager.GetComponent<pdfStorage>();


            GetComponent<Image>().color = new Color(183, 183, 183, 255);

            foreach (Transform obj in DocumentGroup.GetComponentsInChildren<Transform>())
            {
                if (obj.name.Contains("DocumentButton") && obj.name != this.name)
                {
                    obj.GetComponent<Image>().color = new Color(255, 255, 255, 255);

                }
            }

            storage.setActiveDocument(document);
        }
        else
        {
            DrawingTextureManager texMan = manager.GetComponent<DrawingTextureManager>();

            switch (document)
            {
                case "new page":
                    texMan.CreateNewSlide();
                    break;
                case "toggle page":
                    texMan.ToggleDisplay();
                    break;
                case "page left":
                    texMan.MoveSlide(SlideChange.left);
                    break;
                case "page right":
                    texMan.MoveSlide(SlideChange.right);
                    break;
                case "delete page":
                    texMan.DeleteCurrentSlide();
                    break;
                case "export page":
                    texMan.ExportTextures();
                    break;

            }
        }
        
    }

    private void Start()
    {
        timer = GetComponentInParent<ButtonPressTimer>();
    }
}