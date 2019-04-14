using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenu
{
    GameObject GetGameObject();
    void DestroyMenu();
    void DestroyObject();
    bool IsHidden();
}
