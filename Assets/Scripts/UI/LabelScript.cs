using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Doozy.Engine.UI;
using TMPro;

public class LabelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text text;
    public Color color;
    public Color actionColor;
    public bool pressed = false;
    public UIView view;
    public DayNightController dayNightController;

    protected Image image;

    private const float hourModif = 1f / 24f;

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
        if (this.gameObject.GetComponent<RectTransform>() != null)
        {
            Camera.main.GetComponent<CameraController>().zoomEnabled = false;
        }
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
        Camera.main.GetComponent<CameraController>().zoomEnabled = true;
        if (!this.pressed)
        {
            if (this.text != null)
                this.text.color = this.color;
            if (this.image != null)
                this.image.color = this.color;
        }
    }

    public void UpdateHour(bool add = true)
    {
        UIButton btn = this.GetComponent<UIButton>();
        if (btn != null)
        {
            TextMeshProUGUI txt = btn.TextMeshProLabel;
            float vHour = dayNightController.targetTime;

            if (add)
                vHour = (vHour + hourModif) % 1f;
            else
            {
                vHour -= hourModif;
                if (vHour < 0)
                    vHour = 1 + vHour;
            }

            int hour = (int)(vHour * 24f);
            if (hour >= 10)
                txt.SetText("{0} : 00", hour);
            else
                txt.SetText("0{0} : 00", hour);
            this.dayNightController.SetTimeOfDay(vHour);
        }
    }
}