using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class ViewController : MonoBehaviour
{
    public UIButton[] buttons;

    public void ToogleButtons()
    {
        foreach(UIButton button in buttons)
        {
            if (button.GetComponent<LabelScript>().pressed
                && !button.IsSelected && button.IsActive())
                button.ExecuteClick();
        }
    }
}
