using Doozy.Engine.UI;
using UnityEngine;
using TMPro;

public class TutoObject : MonoBehaviour
{
    public UIButton[] buttons;
    public UIView[] views;
    public string[] instructions;
    public int progressIndex;


    public void PlayTutorial()
    {
        if (progressIndex < instructions.Length)
        {
            Debug.Log("PROGRESS INDEX " + progressIndex);
            buttons[progressIndex].ExecuteClick();
            ++progressIndex;
        }
    }

    public void PreviousTutorial()
    {
        Debug.Log("PROGRESS INDEX " + progressIndex);
        if (progressIndex > 0)
            buttons[progressIndex].ExecuteClick();
        if (progressIndex > 0)
            --progressIndex;
    }
}
