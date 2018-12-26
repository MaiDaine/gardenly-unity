using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LabelScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text text;

    protected Image image;
    protected bool pressed;
    // Start is called before the first frame update
    void Start()
    {
        image = transform.GetComponent<Image>();
        pressed = false;
    }


    public void isPressed()
    {
        if (pressed)
            pressed = false;
        else
            pressed = true;
    }
   
    public void ChangeColor()
    {
        Debug.Log("CHANGE " + image.color + " " + text.color);
        isPressed();
        if (pressed)
        {
            text.color = Color.red;
            image.color = Color.red;
        }
        else
        {
            text.color = Color.white;
            image.color = Color.white;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!pressed)
        {
            text.color = Color.red;
            image.color = Color.red;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!pressed)
        {
            text.color = Color.white;
            image.color = Color.white;
        }
    }
}
