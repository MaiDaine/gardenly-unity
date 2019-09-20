using Doozy.Engine.UI;
using UnityEngine;
using TMPro;

public class TutoObject : MonoBehaviour
{
    public UIButton[] buttons;
    public UIView[] views;
    public TextMeshProUGUI title;
    public string[] instructions;
    public int progressIndex = 0;


    public void start()
    {
        title.text = GetComponent<UIButton>().TextMeshProLabel.text;
    }

    public void playTutorial()
    {
        buttons[progressIndex].ExecuteClick();
    }

    public void previousTutorial()
    {
        if (progressIndex > 0)
            --progressIndex;
    }

    public void nextTutorial()
    {
        if (progressIndex < buttons.Length)
            ++progressIndex;
    }
}
