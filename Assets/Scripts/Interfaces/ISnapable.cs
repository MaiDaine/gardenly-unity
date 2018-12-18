using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISnapable
{
    GameObject GetGameObject();
    bool FindSnapPoint(ref Vector3 currentPos, float snapDistance);
    bool isLinkable();
}