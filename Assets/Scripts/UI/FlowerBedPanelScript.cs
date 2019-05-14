using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlowerBedPanelScript : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Dropdown typeDropDown;

    private bool inputDisable = false;

    private void OnDisable()
    {
        if (this.nameInputField != null && this.typeDropDown != null)
            Camera.main.GetComponentInChildren<CameraController>().inputEnabled = true;
    }

    private void LateUpdate()
    {
        if (this.typeDropDown != null && this.typeDropDown.IsActive())
        {
            if (this.typeDropDown.IsExpanded)
            {
                Camera.main.GetComponentInChildren<CameraController>().inputEnabled = false;
            }
           /* else
            {
                ValidateTypeChange();
            }*/
        }
    }

    public void SetType()
    {
        UIController uIController = Camera.main.GetComponentInChildren<UIController>();

        if (uIController.GetFlowerBed() != null && uIController.GetFlowerBed().soilType != "PLACEHOLDER")
        {
            foreach (TMP_Dropdown.OptionData data in this.typeDropDown.options)
            {
                Debug.Log("SET");
                if (data.text == uIController.GetFlowerBed().soilType)
                {
                    this.typeDropDown.value = this.typeDropDown.options.IndexOf(data);
                    Debug.Log(this.typeDropDown.options.IndexOf(data) + " " + uIController.GetFlowerBed().soilType);
                }
            }
        }
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
