using Doozy.Engine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutoObject : MonoBehaviour
{
    public UIButton[] buttons;
    public UIView[] views;
    public ViewController listener;
    public Color activeColor = new Color(65, 159, 59);
    public Color[] refColor;
    public Color[] componentRefColor;
    public Transform[] components;
    public string[] instructions;
    public int progressIndex;
    public int componentIndex = 0;


    private void SetTutorialButton(UIButton button)
    {
        if (!button.GetComponent<LabelScript>().pressed)
            button.ExecuteClick();
        refColor[progressIndex] = button.GetComponent<LabelScript>().color;
        button.GetComponent<Image>().color = activeColor;
    }


    public void PlayTutorial()
    {
        if (progressIndex < instructions.Length && progressIndex < buttons.Length)
        {
            if (buttons[progressIndex] != null)
            {
                SetTutorialButton(buttons[progressIndex]);
            }
            else if (listener != null && buttons[progressIndex] == null)
            {
                SetTutorialButton(listener.dynButtons[0]);
                buttons[progressIndex] = listener.dynButtons[0];
            }
            else
            {
                componentRefColor[componentIndex] = components[componentIndex].GetComponent<Image>().color;
                components[componentIndex].GetComponent<Image>().color = activeColor;
                ++componentIndex;
            }
            ++progressIndex;
        }
    }

    public void PreviousTutorial()
    {
        if (progressIndex > 0 && progressIndex < buttons.Length && buttons[progressIndex - 1] != null)
        {
           Debug.Log("CLICK " + buttons[progressIndex - 1].name);
          buttons[progressIndex - 1].ExecuteClick();
        }
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
    }
}
