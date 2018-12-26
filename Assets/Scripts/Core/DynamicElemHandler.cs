using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicElemHandler : GhostHandler
{
    public  Transform windowPreview;

    protected Transform previewUI;
    protected MenuScript menu;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 1;
        previewUI = null;
    }

    void SetSize(Transform obj, float width, float height)
    {
        RectTransform rect;

        rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);
    }

    void    SetPosition(Transform obj, float x, float y, float z)
    {
        Transform child = obj;
        child.position = new Vector3(x, y, z);
    }

    void    SpawnWindow()
    {
        Canvas canvas;

        previewUI = Instantiate(windowPreview, Vector3.zero, Quaternion.identity);
        previewUI.transform.position = transform.position;
        previewUI.transform.SetParent(transform, false);

        SetSize(previewUI, 20, 10);

        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        SetPosition(previewUI.GetChild(0), transform.position.x, transform.position.y + 20, transform.position.z);
        SetSize(previewUI.GetChild(0), 20, 10);

        SetPosition(previewUI.GetChild(0).GetChild(0), transform.position.x + 16, transform.position.y + 23, transform.position.z);
        SetSize(previewUI.GetChild(0).GetChild(0), 1f, 1f);

        SetPosition(previewUI.GetChild(0).GetChild(1), transform.position.x - 7, transform.position.y + 19, transform.position.z);
        SetSize(previewUI.GetChild(0).GetChild(1), 5, 10);

        menu = previewUI.GetComponent<MenuScript>();
        menu.SetGhostRef(this);    
    }

    //ISELECTABLE
    public override void Select()
    {
        if (previewUI == null)
            SpawnWindow();
        else if (!previewUI.gameObject.activeSelf)
            previewUI.gameObject.SetActive(true);
    }
}
