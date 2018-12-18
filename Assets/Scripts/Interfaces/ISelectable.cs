using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    GameObject GetGameObject();
    void Select();
    List<ISelectable> SelectWithNeighbor();
    void DeSelect();
    void AddNeighbor(ISelectable item);
    void RemoveFromNeighbor(ISelectable item);
}