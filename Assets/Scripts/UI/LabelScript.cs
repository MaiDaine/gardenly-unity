using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Doozy.Engine.UI;

public class LabelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text text;
    public Color color;
    public Color actionColor;
    public bool pressed = false;
    public UIView view;

    protected Image image;
    

    private void Start()
    {
        this.image = transform.GetComponent<Image>();
        if (this.image != null)
            this.image.color = this.color;
    }


    public void IsPressed()
    {
        this.pressed = !this.pressed;
    }

    public void ChangeColor()
    {
        IsPressed();
        if (this.pressed)
        {
            if (this.text != null)
                this.text.color = this.actionColor;
            if (this.image != null)
                this.image.color = this.actionColor;
        }
        else
        {
            if (this.text != null)
                this.text.color = this.color;
            if (this.image != null)
                this.image.color = this.color;
        }
    }

    public void ResetColor()
    {
        if (this.text != null)
            this.text.color = this.color;
        if (this.image != null)
            this.image.color = this.color;
        this.pressed = false;
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
}