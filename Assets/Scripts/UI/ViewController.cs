using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class ViewController : MonoBehaviour
{
    public UIButton[] buttons;

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
}
