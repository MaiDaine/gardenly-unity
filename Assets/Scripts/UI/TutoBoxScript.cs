using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class TutoBoxScript : MonoBehaviour
{
    public UIView tutoPanel;
    public Transform content;
    public TextMeshProUGUI tutorialText;
    public static bool isOn = true;

    public void SetTutorial(string msg)
    {
        tutoPanel.Show();
        tutorialText.text = msg;
    }

    public void SwitchState()
    {
        isOn = !isOn;
    }
}
