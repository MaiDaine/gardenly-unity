using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBedElement : GhostHandler, ISelectable, ISerializable
{
    public enum FlowerBedElementType { Flower01 };

    public SerializationController.ItemType type;
    public FlowerBedElementType subType;
    public Vector3 correctedRotation;

    private SerializableItem serializableItem;

    void Start()
    {
        this.transform.eulerAngles += correctedRotation;
        if (ConstructionController.instance.flowerbedCount < 1)
        {
            ErrorHandler.instance.ErrorMessage("You must place a flowerbed first");
            ConstructionController.instance.Cancel();
        }
    }

    public override void Select(ConstructionController.ConstructionState state)
    {
        //if (state == ConstructionController.ConstructionState.Off)
            //TODO : interface
    }

    public override void DeSelect()
    {
        //TODO : interface
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
