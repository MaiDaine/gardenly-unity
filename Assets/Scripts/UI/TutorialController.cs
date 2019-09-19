using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class TutorialController : MonoBehaviour
{
    public UIView[] tutoSubMenu;
    public UIButton[] navButtons;

    private bool tutoActivate = false;

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
}
