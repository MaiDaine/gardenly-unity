using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMenu : MonoBehaviour
{
    public bool state = false;
    public bool isChild = false;

    void Start()
    {
        gameObject.SetActive(state);
    }

    public void ChangeState()
    {
        gameObject.SetActive(!state);
        state = !state;
    }

    public void SetState(bool state)
    {
        gameObject.SetActive(state);
        this.state = state;
    }

    public void MoveSubMenu(Transform gardenMenu)
    {
        RectTransform rectTransform = gardenMenu.GetComponent<RectTransform>();
        Vector3 tmpPos;
        ConstructionMenu[] tmpMenus = this.GetComponentsInChildren<ConstructionMenu>();

        tmpPos = rectTransform.position;
        if (tmpMenus != null && tmpMenus.Length > 1)
        {
            foreach (ConstructionMenu menu in tmpMenus)
            {
                tmpPos.y += 90;
                if (menu.isChild)
                    menu.ChangeState();
            }
        }
        else
        {
            if (state)
                tmpPos.y -= 90;
            else
                tmpPos.y += 90;
        }
        rectTransform.position = tmpPos;
    }
}
