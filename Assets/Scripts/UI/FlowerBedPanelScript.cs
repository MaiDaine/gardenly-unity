using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowerBedPanelScript : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_InputField typeInputField;

    private void OnDisable()
    {
        if (nameInputField != null && typeInputField && (nameInputField.IsActive() || typeInputField.IsActive()))
            Camera.main.GetComponentInChildren<CameraController>().inputEnabled = true;
    }


    public void EnableCameraMovement(bool state)
    {
        Camera.main.GetComponentInChildren<CameraController>().inputEnabled = state;
    }

    public void ValidateNameChange()
    {
        Camera.main.GetComponentInChildren<UIController>().UpdateNameFlowerBed(nameInputField.text);
    }

    public void ValidateTypeChange()
    {
        Camera.main.GetComponentInChildren<UIController>().UpdateTypeFlowerBed(typeInputField.text);
    }
}
