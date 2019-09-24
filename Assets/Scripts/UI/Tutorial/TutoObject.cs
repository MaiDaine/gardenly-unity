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
    public TMP_InputField inputField;
    public string[] instructions;
    public int progressIndex;
    public int componentIndex = 0;

    public static bool waitForBuild = false;


    private void SetTutorialButton(UIButton button)
    {
        if (!button.GetComponent<LabelScript>().pressed && button.IsActive())
        {
            button.ExecuteClick();
            button.GetComponent<LabelScript>().pressed = true;
        }
        refColor[progressIndex] = button.GetComponent<LabelScript>().color;
        button.GetComponent<Image>().color = activeColor;
    }

    private void ResetTutorialButton(UIButton button)
    {
        if (button.IsActive())
            button.ExecuteClick();
        button.GetComponent<LabelScript>().pressed = false;
    }


    public void PlayTutorial()
    {
        if (progressIndex < instructions.Length && progressIndex < buttons.Length)
        {
            if (buttons[progressIndex] != null)
                SetTutorialButton(buttons[progressIndex]);
            else if (listener != null && buttons[progressIndex] == null)
            {
                SetTutorialButton(listener.dynButtons[0]);
                buttons[progressIndex] = listener.dynButtons[0];
            }
            else if (inputField != null && buttons[progressIndex] == null)
                inputField.text = "P";
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
        if (progressIndex > 0 && progressIndex < buttons.Length)
        {
            if (buttons[progressIndex] != null && buttons[progressIndex].GetComponent<LabelScript>().pressed)
                ResetTutorialButton(buttons[progressIndex]);
            if (buttons[progressIndex - 1] != null && buttons[progressIndex - 1].GetComponent<LabelScript>().pressed)
                ResetTutorialButton(buttons[progressIndex - 1]);
        }
        if (progressIndex > 0)
        {
            --progressIndex;
            if (buttons[progressIndex] != null)
                buttons[progressIndex].GetComponent<Image>().color = refColor[progressIndex];
            else if (inputField != null && inputField.IsActive())
                inputField.text = "";
            else if (components.Length > 0 && componentIndex < components.Length)
            {
                --componentIndex;
                components[componentIndex].GetComponent<Image>().color = componentRefColor[componentIndex];
            }
        }
    }
}
