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

    /*public void ToogleButtons()
    {
        Debug.Log(this.buttons[0].name);
        Debug.Log(this.buttons[1].name);
        Debug.Log(this.buttons[2].name);
        foreach (UIButton button in this.buttons)
        {
            if (button.isActiveAndEnabled 
                && !button.IsSelected
                && button.GetComponentInChildren<ConstructionMenu>() != null
                && button.GetComponentInChildren<ConstructionMenu>().state)
                button.ExecuteClick();
        }
    }*/

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
                }
            }
        }
    }

   /* public void DesactivateButtons()
    {
        if (startCount == true)
            return;
        if (!this.isToogle)
        {
            startCount = !startCount;
            this.isToogle = !this.isToogle;
            return;
        }
        foreach (UIButton button in this.buttons)
        {        
            button.gameObject.SetActive(true); 
        }
        this.isToogle = !this.isToogle;
    }*/

    public void AddPlants()
    {
        foreach (PlantData plant in ReactProxy.instance.externalData.plants[this.plantType].Values)
        {
            GameObject obj = Instantiate(this.plantButton, view.transform);
            ButtonScript buttonScript = obj.GetComponent<ButtonScript>();
            UIButton btn = obj.GetComponent<UIButton>();
            
            dynamicButtonListener.GetComponent<ViewController>().dynButtons.Add(btn);
            btn.TextMeshProLabel.text = plant.name;
            buttonScript.SetGhost(this.plantType);
        }
    }
}
