using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SerializationData
{
    public SerializationController.ItemType type;
    public string serializedData;
}

[Serializable]
public struct SerializedData
{
    public SerializationData[] data;
}