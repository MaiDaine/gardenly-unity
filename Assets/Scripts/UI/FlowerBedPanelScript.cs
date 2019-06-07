using UnityEngine;
using TMPro;

// Manage name and type changes
public class FlowerBedPanelScript : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown typeDropDown;

    private void OnDisable()
    {
        EnableCameraMovement(true);
    }

    public void EnableCameraMovement(bool state)
    {
        if (Camera.main != null)
            Camera.main.GetComponent<CameraController>().inputEnabled = state;
    }

    public void ValidateNameChange()
    {
        Camera.main.GetComponent<UIController>().UpdateNameFlowerBed(nameInputField.text);
    }

    public void ValidateTypeChange()
    {
        EnableCameraMovement(true);
        Camera.main.GetComponent<UIController>().UpdateTypeFlowerBed(typeDropDown.options[typeDropDown.value].text);
    }
}
