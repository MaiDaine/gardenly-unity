using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHandler : DynamicElemHandler
{
    void    Awake()
    {
        Debug.Log("AWAKE");
        data.objectName = "Tree";
        data.description = "this is a simple description of this Tree";
        data.solidity = 0.4f;
        data.water = 0.7f;
        data.sunshine = 1f;
        data.age = 0;
    }

    void    Start()
    {
        Debug.Log("START");
    }

    public override ObjectsData GetData()
    {
        return this.data;
    }

}
