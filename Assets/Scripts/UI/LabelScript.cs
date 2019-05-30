using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Doozy.Engine.UI;

// Manage color change of the ref OnPointerEnter / Exit and when click
public class LabelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text text;
    public Color color;
    public Color actionColor;
    public UIView view;
    public bool pressed = false;

    protected Image image;

    private void Start()
    {
        image = transform.GetComponent<Image>();
        if (image != null)
            image.color = color;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!pressed)
        {
            if (text != null)
                text.color = actionColor;
            if (image != null)
                image.color = actionColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pressed)
        {
            if (text != null)
                text.color = color;
            if (image != null)
                image.color = color;
        }
    }

    public void TooglePressed()
    {
        pressed = !pressed;
    }

    public void ChangeColor()
    {
        TooglePressed();
        Color tmp;

        if (pressed)
            tmp = actionColor;
        else
            tmp = color;

        if (text != null)
            text.color = tmp;
        if (image != null)
            image.color = tmp;
    }

    public void ResetColor()
    {
        if (text != null)
            text.color = color;
        if (image != null)
            image.color = color;
        pressed = false;
    }
}