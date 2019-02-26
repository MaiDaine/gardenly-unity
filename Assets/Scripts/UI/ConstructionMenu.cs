using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionMenu : MonoBehaviour
{
    public bool state = false;

    public static ConstructionMenu instance = null;

    void Awake()
    {
      //if (instance == null)
      instance = this;
      //else if (instance != this)
        //Destroy(instance);
    }

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
        Debug.Log("STATE " + state);
        if (tmpMenus != null && tmpMenus.Length > 1)
        {
          foreach (ConstructionMenu menu in tmpMenus)
          {
            if (menu.state)
              {
                Debug.Log("MENU " + menu.transform.parent.name + "MSTATE " + menu.state);
                  menu.state = false;
                  if (state)
                    tmpPos.y -= 90;
                  else
                    tmpPos.y += 90;
              }
          }
        }
        if (state)
            tmpPos.y -= 90;
        else
            tmpPos.y += 90;
        rectTransform.position = tmpPos;
    }
}
