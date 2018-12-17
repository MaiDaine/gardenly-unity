using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    GameObject GetGameObject();
    void Select();
    void DeSelect();
}