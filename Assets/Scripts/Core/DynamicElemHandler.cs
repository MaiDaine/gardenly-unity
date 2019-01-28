using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicElemHandler : GhostHandler
{
    public Transform windowPreview;
    public bool startRotate = false;
    public Vector3 currentPosition;

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

    void SetPosition(Transform obj, float x, float y, float z)
    {
        Transform child = obj;
        child.position = new Vector3(x, y, z);
    }

    void    SpawnWindow()
    {
        Canvas canvas;
        Vector3 position;

        position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);

        previewUI = Instantiate(windowPreview, position, Quaternion.identity, transform);

        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        menu = previewUI.GetComponent<MenuScript>();
        menu.SetGhostRef(this);
    }

    void OnMouseDrag()
    {
        if (menu != null && menu.rotateState)
            menu.RotateGhost();
    }


    //ISELECTABLE
    public override void Select(ConstructionController.ConstructionState state)
    {
        if (previewUI == null)
            SpawnWindow();
        else if (!previewUI.gameObject.activeSelf)
            previewUI.gameObject.SetActive(true);
    }
}