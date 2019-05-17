using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenu
{
    GameObject GetGameObject();
    void DestroyMenu(bool spawn = false);
    void DestroyObject();
    bool IsHidden();
}
