using UnityEngine;
using UnityEngine.EventSystems;

// Disable camera scroll on pointer enter
public class ScrollViewScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void OnDisable()
    {
        Debug.Log("DISABLE");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Camera.main.GetComponentInChildren<CameraController>().zoomEnabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Camera.main.GetComponentInChildren<CameraController>().zoomEnabled = true;
    }
}
