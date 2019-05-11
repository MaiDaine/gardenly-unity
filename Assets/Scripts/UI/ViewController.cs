using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.UI;
using TMPro;

public class ViewController : MonoBehaviour
{
    public UIButton[] buttons;
    public Transform view;
    public GameObject plantButton;
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

    public void ToogleButtons()
    {
        foreach (UIButton button in buttons)
        {
            if (button != null
                && button.isActiveAndEnabled 
                && !button.IsSelected
                && button.GetComponent<LabelScript>() != null
                && button.GetComponent<LabelScript>().view.IsVisible)
                button.ExecuteClick();
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
                    labelScript.ResetColor();
                }
            }
        }
    }

    public void DesactivateButtons()
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
    }

    public void AddPlants()
    {
        foreach (PlantData plant in ReactProxy.instance.externalData.plants[this.plantType].Values)
        {
            GameObject obj = Instantiate(this.plantButton, view.transform);
            ButtonScript buttonScript = obj.GetComponent<ButtonScript>();
            UIButton btn = obj.GetComponent<UIButton>();
            btn.TextMeshProLabel.text = plant.name;
            buttonScript.SetGhost(null, plant.name);
            buttonScript.ghosts[buttonScript.idxObject].SetData(plant);
            Debug.Log("NAME " + plant.name);
            Debug.Log("NAME " + plant.phRangeHigh);
            Debug.Log("NAME " + plant.phRangeLow);
            Debug.Log("NAME " + plant.sunNeed);
            Debug.Log("NAME " + plant.waterNeed);
            Debug.Log("NAME " + plant.rusticity);
            // this.plantButton.GetComponentInChildren<Image>() = plant.imageUrl

        }
    }
}
