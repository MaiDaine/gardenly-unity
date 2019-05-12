using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

public class TutoBoxScript : MonoBehaviour
{
    public UIView tutoPanel;
    public Transform content;
    public TextMeshProUGUI tutorialText;
    public bool isOn;

    public void SetTutorial(string msg)
    {
        if (isOn)
        {
            tutoPanel.Show();
            tutorialText.text = msg;
        }
    }

    public void SwitchState()
    {
        isOn = !isOn;
    }
}
