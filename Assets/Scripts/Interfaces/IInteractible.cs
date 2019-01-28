using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractible
{
    GameObject GetGameObject();
    void Activate();
    void DeActivate();
    void EndDrag();
    void DragClick(Vector3 mousePosition);
}
