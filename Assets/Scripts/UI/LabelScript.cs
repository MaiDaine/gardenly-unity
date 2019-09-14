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
    public Sprite initial;
    public Sprite updated;
    public UIView view;
    public bool pressed = false;

    protected Image image;

    private void Start()
    {
        image = transform.GetComponent<Image>();
        if (image != null)
            image.color = color;
    }

    private void SetColor(Color color)
    {
        if (text != null)
            text.color = color;
        if (image != null)
            image.color = color;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!pressed)
            SetColor(actionColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pressed)
            SetColor(color);
    }

    public void TooglePressed()
    {
        pressed = !pressed;
    }

    public void ChangeColor()
    {
        Color tmp;

        TooglePressed();
        if (pressed)
            tmp = actionColor;
        else
            tmp = color;

        SetColor(tmp);
    }

    public void ResetColor()
    {
        SetColor(color);
        pressed = false;
    }

    public void UpdateIcon()
    {
        if (pressed)
            image.sprite = updated;
        else
            image.sprite = initial;
    }
}