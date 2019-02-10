using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBedElement : GhostHandler, ISerializable //TODO ADD SELECT
{
    public enum FlowerBedElementType { Flower01 };

    public SerializationController.ItemType type;
    public FlowerBedElementType subType;
    public Vector3 correctedRotation;

    private SerializableItem serializableItem;

    void Start()
    {
        this.transform.eulerAngles += correctedRotation;
    }

    //Serialization
    [Serializable]
    public struct SerializableItem
    {
        public FlowerBedElementType subType;
        public Transform transform;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;

        serializableItem.transform = this.transform;
        serializableItem.subType = subType;
        tmp.type = SerializationController.ItemType.FlowerBedElement;
        tmp.serializedData = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        this.transform.position = serializableItem.transform.position;
        this.transform.rotation = serializableItem.transform.rotation;
        this.transform.localScale = serializableItem.transform.localScale;
    }
}
