using UnityEngine;
using Doozy.Engine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public UIView[] tutoSubMenu;
    public UIButton[] navButtons;
    public TextMeshProUGUI title;
    public TextMeshProUGUI body;

    private bool tutoActivate = false;
    private TutoObject tutoObject = null;

    //Play Tutorial management
    public void SetTutorial(TutoObject tutorial)
    {
        tutoObject = tutorial;
        tutoObject.progressIndex = 0;
        tutoObject.componentIndex = 0;
        title.text = tutoObject.GetComponent<UIButton>().TextMeshProLabel.text;
        body.text = tutoObject.instructions[tutoObject.progressIndex];
        navButtons[1].DisableButton();
        if (!navButtons[2].Interactable)
            navButtons[2].EnableButton();
        if (tutoObject.instructions.Length == 1)
        {
            navButtons[2].DisableButton();
            tutoObject.buttons[0].GetComponent<Image>().color = tutoObject.activeColor;
        }
    }

    public void PlayTutorial()
    {
        if (tutoObject.waitForBuild)
        {
            MessageHandler.instance.ErrorMessage("tutorial_error");
            return;
        }
        tutoObject.PlayTutorial();
        body.text = tutoObject.instructions[tutoObject.progressIndex];
        if (tutoObject.progressIndex + 1 >= tutoObject.instructions.Length)
            navButtons[2].DisableButton();
        if (!navButtons[1].Interactable && tutoObject.progressIndex > 0)
            navButtons[1].EnableButton();
    }

    public void PreviousTutorial()
    {
        if (tutoObject.waitForBuild)
        {
            MessageHandler.instance.ErrorMessage("tutorial_error");
            return;
        }
        tutoObject.PreviousTutorial();
        body.text = tutoObject.instructions[tutoObject.progressIndex];
        if (tutoObject.progressIndex <= 0)
            navButtons[1].DisableButton();
        if (!navButtons[2].Interactable && tutoObject.progressIndex < tutoObject.instructions.Length)
            navButtons[2].EnableButton();
    }

    public void CloseTutorial()
    {
        if (tutoObject != null)
        {
            if (tutoObject.buttons.Length > 0 && tutoObject.instructions.Length > 1)
            {
                for (int cnt = tutoObject.progressIndex; cnt >= 0; cnt--)
                {
                    if (cnt < tutoObject.buttons.Length && tutoObject.buttons[cnt] != null && tutoObject.buttons[cnt].IsActive() && tutoObject.buttons[cnt].GetComponent<LabelScript>().pressed)
                    {
                        tutoObject.buttons[cnt].ExecuteClick();
                        tutoObject.buttons[cnt].GetComponent<LabelScript>().pressed = false;
                    }
                }
                for (int cnt = tutoObject.componentIndex; cnt >= 0; cnt--)
                {
                    if (cnt < tutoObject.components.Length && tutoObject.components[cnt] != null)
                        tutoObject.components[cnt].GetComponent<Image>().color = tutoObject.componentRefColor[cnt];
                }
                if (tutoObject.inputField != null)
                    tutoObject.inputField.text = "";
            }
            if (tutoObject.instructions.Length == 1)
                tutoObject.buttons[0].GetComponent<Image>().color = tutoObject.buttons[0].GetComponent<LabelScript>().color;
        }
    }

    //UI management
    public void CloseSubMenu()
    {
        foreach (UIView view in tutoSubMenu)
        {
            if (view.IsVisible)
                view.Hide();
        }
    }

    public void ToogleNavButtons()
    {
        foreach (UIButton button in navButtons)
        {
            button.gameObject.SetActive(!button.IsActive());
        }
    }

    public void ToogleTutoState()
    {
        tutoActivate = !tutoActivate;
    }

    public bool GetTutoState()
    {
        return tutoActivate;
    }

    public void OnTutoBlock()
    {
        if (tutoObject != null)
            tutoObject.waitForBuild = !tutoObject.waitForBuild;
    }
}
