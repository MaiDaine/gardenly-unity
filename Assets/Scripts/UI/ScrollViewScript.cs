using UnityEngine;
using UnityEngine.EventSystems;

// Behaviour of scroll view
public class ScrollViewScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Set the soil type when the dropdown is close
    private void OnDisable()
    {
        FlowerBedPanelScript panel = GetComponentInParent<FlowerBedPanelScript>();
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraController>().zoomEnabled = true;
            if (gameObject.name == "Dropdown List" && panel != null && Camera.main.GetComponent<UIController>().GetFlowerBed() != null)
                panel.ValidateTypeChange();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraController>().zoomEnabled = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraController>().zoomEnabled = true;
        }
    }
}
