using UnityEngine;
using UnityEngine.EventSystems;

// Disable camera scroll on pointer enter
public class ScrollViewScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void OnDisable()
    {
        FlowerBedPanelScript panel = this.GetComponentInParent<FlowerBedPanelScript>();
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraController>().zoomEnabled = true;
            if (this.gameObject.name == "Dropdown List" && panel != null && Camera.main.GetComponent<UIController>().GetFlowerBed() != null)
                panel.ValidateTypeChange();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Camera.main.GetComponent<CameraController>().zoomEnabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Camera.main.GetComponent<CameraController>().zoomEnabled = true;
    }
}
