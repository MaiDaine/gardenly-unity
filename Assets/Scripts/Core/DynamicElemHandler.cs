using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicElemHandler : GhostHandler
{
    public  Transform windowPreview;

    protected bool isSelected;
    protected Transform previewUI;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 1;
    }

    //ISELECTABLE
    public override void Select()
    {
        Debug.Log("SELECT");
        previewUI = Instantiate(windowPreview, Vector3.zero, Quaternion.identity);
        previewUI.transform.position = transform.position;
        previewUI.transform.SetParent(transform, false);
        Transform panel = previewUI.GetChild(0);
        
        RectTransform rect = previewUI.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(40, 20);
        panel.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);
        isSelected = true;
    }

    public override void DeSelect()
    {
        Debug.Log("DESELECT");
        isSelected = false;
        Destroy(previewUI.gameObject);
    }
}
