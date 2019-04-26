using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerBedPanelScript : MonoBehaviour
{
    public InputField inputField;

    private void OnDisable()
    {
        Camera.main.GetComponentInChildren<CameraController>().inputEnabled = true;
    }


    public void EnableCameraMovement(bool state)
    {
        Debug.Log("Enable");
        Camera.main.GetComponentInChildren<CameraController>().inputEnabled = state;
    }

    public void ValidateChange()
    {
        Camera.main.GetComponentInChildren<UIController>().UpdateFlowerBedDataPanel(inputField.text);
    }
}
