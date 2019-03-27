using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenData : MonoBehaviour, ISerializable
{
    public string gardenName;
    private Vector2[] bounds = new Vector2[2] { new Vector2(0, 0), new Vector2(100, 100) };

    private void Start()
    {
        SerializationController.instance.AddToList(this);
    }

    private void OnDestroy()
    {
        SerializationController.instance.RemoveFromList(this);
    }

    //Serialization
    [Serializable]
    public struct SerializedGardenData
    {
        public string name;
        public Vector2[] bounds;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;
        SerializedGardenData gardenData;

        tmp.type = SerializationController.ItemType.GardenData;
        gardenData.name = gardenName;
        gardenData.bounds = bounds;
        tmp.data = JsonUtility.ToJson(gardenData);
        return tmp;
    }

    public void DeSerialize(string json)
    {
        SerializedGardenData gardenData = JsonUtility.FromJson<SerializedGardenData>(json);

        gardenName = gardenData.name;
    }
}
