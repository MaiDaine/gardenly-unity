﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.UI;
using TMPro;

public class ViewController : MonoBehaviour
{
    // FlowerBed after click define soil type + tutorial world space object
    public TextMeshProUGUI[] labels;
    public UIButton[] buttons;
    public UIView plantView;
    public List<UIButton> dynButtons;
    public Transform view;
    public GameObject plantButton;
    public UIButtonListener dynamicButtonListener;
    public string plantType;
    public bool isPressed = false;

    public void ResetButtons()
    {
        foreach(UIButton button in this.buttons)
        {
            if (!button.IsSelected && button.IsActive())
            {
                LabelScript[] tmp = button.GetComponentsInChildren<LabelScript>();
                ConstructionMenu constructionMenu = button.GetComponentInChildren<ConstructionMenu>();
                if (constructionMenu != null)
                    constructionMenu.ChangeState();
                foreach (LabelScript labelScript in tmp)
                {
                    labelScript.ResetColor();
                    if (labelScript.view != null && labelScript.view.IsVisible)
                        labelScript.view.Hide();
                }
            }   
        }
    }

    public void ResetDynButtons()
    {
        foreach (UIButton button in this.dynButtons)
        {
            if (!button.IsSelected && button.IsActive())
            {
                LabelScript[] tmp = button.GetComponentsInChildren<LabelScript>();
                ConstructionMenu constructionMenu = button.GetComponentInChildren<ConstructionMenu>();
                if (constructionMenu != null)
                    constructionMenu.ChangeState();
                foreach (LabelScript labelScript in tmp)
                {
                    labelScript.ResetColor();
                    if (labelScript.view != null && labelScript.view.IsVisible)
                        labelScript.view.Hide();
                }
            }
        }
    }

    public void SwitchState()
    {
        this.isPressed = !this.isPressed;
    }

    public void ExtendMenuMode()
    {
        RectTransform rect = this.GetComponent<RectTransform>();
        this.isPressed = !this.isPressed;

        Camera.main.GetComponent<UIController>().HideViews();
        Camera.main.GetComponent<UIController>().uIButtonListener.GetComponent<ViewController>().ResetButtons();
        foreach (TextMeshProUGUI text in this.labels)
        {
            text.gameObject.SetActive(this.isPressed);
        }
        if (this.isPressed)
            rect.sizeDelta = new Vector2(122.4f, rect.sizeDelta.y);
        else
            rect.sizeDelta = new Vector2(60.82f, rect.sizeDelta.y);
    }
 
    public void AddPlants(UIView viewRef)
    {
        ViewController viewController = dynamicButtonListener.GetComponent<ViewController>(); 
        UIController controller = Camera.main.GetComponent<UIController>();
        
        viewRef.CustomStartAnchoredPosition = new Vector3(- controller.extendMenu.RectTransform.sizeDelta.x + 0.4f,-115, 0);
        if (viewController.dynButtons.Count == 0)
        {
            if (ReactProxy.instance.externalData.plants[this.plantType].Values.Count > 0)
            {
                view.GetComponentInChildren<RawImage>().gameObject.SetActive(false);
                foreach (PlantData plant in ReactProxy.instance.externalData.plants[this.plantType].Values)
                {
                    GameObject obj = Instantiate(this.plantButton, view.transform);
                    ButtonScript buttonScript = obj.GetComponent<ButtonScript>();
                    UIButton btn = obj.GetComponent<UIButton>();

                    viewController.dynButtons.Add(btn);
                    btn.TextMeshProLabel.text = plant.name;
                    buttonScript.SetGhost(this.plantType);
                }
            }
        }
    }
}