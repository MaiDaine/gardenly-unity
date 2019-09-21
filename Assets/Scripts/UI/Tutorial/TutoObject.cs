using Doozy.Engine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutoObject : MonoBehaviour
{
    public UIButton[] buttons;
    public UIView[] views;
    public string[] instructions;
    public int progressIndex;
    public Color activeColor = new Color(65, 159, 59);
    public Color[] refColor;


    public void PlayTutorial()
    {
        if (progressIndex < instructions.Length)
        {
            Debug.Log("PROGRESS INDEX " + progressIndex);
            buttons[progressIndex].ExecuteClick();
            refColor[progressIndex] = buttons[progressIndex].GetComponent<LabelScript>().color;
            buttons[progressIndex].GetComponent<Image>().color = activeColor;
            ++progressIndex;
        }
    }

    public void PreviousTutorial()
    {
        Debug.Log("PROGRESS INDEX " + progressIndex);
        if (progressIndex > 0)
            buttons[progressIndex].ExecuteClick();
        if (progressIndex > 0)
        {
            --progressIndex;
            buttons[progressIndex].GetComponent<Image>().color = refColor[progressIndex];
        }
    }
}
