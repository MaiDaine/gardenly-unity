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
        this.image = transform.GetComponent<Image>();
        if (this.image != null)
            this.image.color = this.color;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!this.pressed)
        {
            if (this.text != null)
                this.text.color = this.actionColor;
            if (this.image != null)
                this.image.color = this.actionColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!this.pressed)
        {
            if (this.text != null)
                this.text.color = this.color;
            if (this.image != null)
                this.image.color = this.color;
        }
    }

    public void IsPressed()
    {
        this.pressed = !this.pressed;
    }

    public void ChangeColor()
    {
        IsPressed();
        Color tmp;

        if (this.pressed)
            tmp = this.actionColor;
        else
            tmp = this.color;

        if (this.text != null)
            this.text.color = tmp;
        if (this.image != null)
            this.image.color = tmp;
    }

    public void ResetColor()
    {
        if (this.text != null)
            this.text.color = this.color;
        if (this.image != null)
            this.image.color = this.color;
        this.pressed = false;
    }
}