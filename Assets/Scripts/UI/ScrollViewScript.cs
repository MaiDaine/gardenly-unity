using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.gameObject.GetComponent<RectTransform>() != null)
        {
            Camera.main.GetComponent<CameraController>().zoomEnabled = false;
        }
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Camera.main.GetComponent<CameraController>().zoomEnabled = true;
    }
}
