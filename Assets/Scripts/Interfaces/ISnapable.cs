using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISnapable
{
    Vector3 FindSnapPoint(Vector3 currentPos, float snapDistance);
}