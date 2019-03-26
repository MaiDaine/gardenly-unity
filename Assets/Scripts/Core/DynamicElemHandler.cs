using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicElemHandler : DefaultStaticElement
{
    public Vector3 currentPosition;

    void Start()
    {
        this.gameObject.layer = 1;
    }
}