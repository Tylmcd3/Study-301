using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[Elixir]
public class buttonCollision : MonoBehaviour
{
    public EventTrigger.TriggerEvent customCallback;

    private void OnTriggerEnter(Collider other)
    {
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        eventData.selectedObject = this.gameObject;

        customCallback.Invoke(eventData);
    }
}
