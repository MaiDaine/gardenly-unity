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
    public int componentIndex = 0;
    public Color activeColor = new Color(65, 159, 59);
    public Color[] refColor;
    public Color[] componentRefColor;
    public Transform[] components;


    public void PlayTutorial()
    {
        if (progressIndex < instructions.Length && progressIndex < buttons.Length)
        {
            if (buttons[progressIndex] != null)
            {
                buttons[progressIndex].ExecuteClick();
                refColor[progressIndex] = buttons[progressIndex].GetComponent<LabelScript>().color;
                buttons[progressIndex].GetComponent<Image>().color = activeColor;
            }
            else
            {
                componentRefColor[componentIndex] = components[componentIndex].GetComponent<Image>().color;
                components[componentIndex].GetComponent<Image>().color = activeColor;
                ++componentIndex;
            }
            ++progressIndex;
            Debug.Log("NEXT PROGRESS INDEX " + progressIndex + "COMPONENT INDEX " + componentIndex);
        }
    }

    public void PreviousTutorial()
    {
        if (progressIndex >= 0 && progressIndex < buttons.Length && buttons[progressIndex] != null)
            buttons[progressIndex].ExecuteClick();
        if (progressIndex > 0)
        {
            --progressIndex;
            if (buttons[progressIndex] != null)
                buttons[progressIndex].GetComponent<Image>().color = refColor[progressIndex];
            else
            {
                --componentIndex;
                components[componentIndex].GetComponent<Image>().color = componentRefColor[componentIndex];
            }
        }
        Debug.Log("PREVIOUS PROGRESS INDEX " + progressIndex + "COMPONENT INDEX " + componentIndex);
    }
}
