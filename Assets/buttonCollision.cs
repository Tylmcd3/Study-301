using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[Elixir]
public class buttonCollision : MonoBehaviour
{
    public bool toggleGroup = false;
    public string document;
    public GameObject DocumentGroup;
    public EventTrigger.TriggerEvent OnTriggerEnterEvent;
    public GameObject manager;
    void OnTriggerEnter(Collider other)
    {
        if (toggleGroup)
        {
            pdfStorage storage = manager.GetComponent<pdfStorage>();
            MelonLoader.MelonLogger.Msg(document);

            GetComponent<Image>().color = new Color(183, 183, 183, 255);

            foreach (Transform obj in DocumentGroup.GetComponentInChildren<Transform>())
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
            BaseEventData eventData = new BaseEventData(EventSystem.current);
            eventData.selectedObject = this.gameObject;
            OnTriggerEnterEvent.Invoke(eventData);
        }


        
    }
}