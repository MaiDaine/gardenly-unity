using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollViewScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        
            Camera.main.GetComponentInChildren<CameraController>().zoomEnabled = false;
      
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
       
            Camera.main.GetComponentInChildren<CameraController>().zoomEnabled = true;
       
    }
}
