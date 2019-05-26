using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.UI;
using TMPro;

// Manage actions (resize / reset buttons colors / Add prefab buttons) on view when button are clicked
public class ViewController : MonoBehaviour
{
    public TextMeshProUGUI[] labels;
    public UIButton[] buttons;
    public UIView plantView;
    public List<UIButton> dynButtons;
    public Transform view;
    public GameObject plantButton;
    public UIButtonListener dynamicButtonListener;
    public string plantType;
    public bool isPressed = false;


    // size view
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
            rect.sizeDelta = new Vector2(200f, rect.sizeDelta.y);
        else
            rect.sizeDelta = new Vector2(61.854f, rect.sizeDelta.y);
    }

    public void SetViewAnchor(UIView viewRef)
    {
        UIController controller = Camera.main.GetComponent<UIController>();
        if (controller != null)
            viewRef.CustomStartAnchoredPosition = new Vector3(-controller.extendMenu.RectTransform.sizeDelta.x + 0.4f, -33.46f, 0);
    }


    // reset buttons
    public void ResetStateButtons(UIButton button)
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

    public void ResetButtons()
    {
        foreach(UIButton button in this.buttons)
        {
            this.ResetStateButtons(button);
        }
    }

    public void ResetDynButtons()
    {
        foreach (UIButton button in this.dynButtons)
        {
            this.ResetStateButtons(button);
        }
    }

    // Add dynamic buttons
    public void AddPlants()
    {
        ViewController viewController = dynamicButtonListener.GetComponent<ViewController>(); 
        UIController controller = Camera.main.GetComponent<UIController>();
        UIView parentView = view.GetComponentInParent<UIView>();
        RawImage img;

        if (viewController.dynButtons.Count == 0)
        {
            string[] plantNames = ReactProxy.instance.GetPlantsType(this.plantType);
            if (plantNames != null)
            {
                img = this.view.GetComponentInChildren<RawImage>();
                if (img != null)
                    this.view.GetComponentInChildren<RawImage>().gameObject.SetActive(false);
                for (int i = 0; i < plantNames.Length; i++)
                {
                    GameObject obj = Instantiate(this.plantButton, view.transform);
                    ButtonScript buttonScript = obj.GetComponent<ButtonScript>();
                    UIButton btn = obj.GetComponent<UIButton>();

                    viewController.dynButtons.Add(btn);
                    buttonScript.SetGhostType(this.plantType);
                    btn.TextMeshProLabel.text = plantNames[i];
                }
            }
        }
    }

    public void SwitchState()
    {
        this.isPressed = !this.isPressed;
    }
}