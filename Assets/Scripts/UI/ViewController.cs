using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.UI;
using TMPro;

public class ViewController : MonoBehaviour
{
    // FlowerBed after click define soil type + tutorial world space object
    public UIButton[] buttons;
    public UIView plantView;
    public List<UIButton> dynButtons;
    public Transform view;
    public GameObject plantButton;
    public UIButtonListener dynamicButtonListener;
    public string plantType;

    protected bool isToogle = false;
    private bool startCount = false;
    private float timer = 0.5f;

    private void Update()
    {
        if (this.startCount)
        {
            this.timer -= Time.deltaTime;

            if (this.timer <= 0)
            {
                foreach (UIButton button in this.buttons)
                {
                    button.gameObject.SetActive(false);
                }
                this.timer = 0.5f;
                this.startCount = false;
            }
        }
    }

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
                    Debug.Log(button.name + " Reset color");
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
                    Debug.Log(button.name + " Reset color");
                    labelScript.ResetColor();
                    if (labelScript.view != null && labelScript.view.IsVisible)
                        labelScript.view.Hide();
                }
            }
        }
    }
 
    public void AddPlants()
    {
        ViewController viewController = dynamicButtonListener.GetComponent<ViewController>();
        if (viewController.dynButtons.Count == 0)
        {
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