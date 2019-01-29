using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicElemHandler : DefaultStaticElement
{
    public Vector3 currentPosition;

   
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 1;
        
    }
}