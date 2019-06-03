using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private void SetTextAndImageColor(Color color)
    {
        if (text != null)
            text.color = color;
        if (image != null)
            image.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!pressed)
            SetTextAndImageColor(actionColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pressed)
            SetTextAndImageColor(color);
    }

    public void TooglePressed()
    {
        pressed = !pressed;
    }

    public void ChangeColor()
    {
        TooglePressed();
        SetTextAndImageColor(pressed ? actionColor : color);
    }

    public void ResetColor()
    {
        SetTextAndImageColor(color);
        pressed = false;
    }
}