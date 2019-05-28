using UnityEngine;
using TMPro;

// Manage name and type changes
public class FlowerBedPanelScript : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown typeDropDown;

    private void OnDisable()
    {
        if (Camera.main != null)
            Camera.main.GetComponentInChildren<CameraController>().inputEnabled = true;
    }

    public void EnableCameraMovement(bool state)
    {
        Camera.main.GetComponentInChildren<CameraController>().inputEnabled = state;
    }

    public void ValidateNameChange()
    {
        Camera.main.GetComponentInChildren<UIController>().UpdateNameFlowerBed(this.nameInputField.text);
    }

    public void ValidateTypeChange()
    {
        Camera.main.GetComponentInChildren<CameraController>().inputEnabled = true;
        Camera.main.GetComponentInChildren<UIController>().UpdateTypeFlowerBed(typeDropDown.options[this.typeDropDown.value].text);
    }
}
