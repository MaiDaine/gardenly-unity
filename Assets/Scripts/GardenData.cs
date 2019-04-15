using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenData : MonoBehaviour, ISerializable
{
    public string gardenName = "PlaceHolder";
    private Vector2[] boundaries = new Vector2[2] { new Vector2(0, 0), new Vector2(100, 100) };

    private void Start()
    {
        SerializationController.instance.SetGardenData(StoreData());
    }

    //Serialization
    [Serializable]
    public struct SerializedGardenData
    {
        public string name;
        public Vector2[] boundaries;
    }

    private SerializedGardenData StoreData()
    {
        SerializedGardenData gardenData;

        gardenData.name = gardenName;
        gardenData.boundaries = boundaries;
        return gardenData;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;

        tmp.type = SerializationController.ItemType.GardenData;
        tmp.data = JsonUtility.ToJson(StoreData());
        return tmp;
    }

    public void DeSerialize(string json)
    {
        SerializedGardenData gardenData = JsonUtility.FromJson<SerializedGardenData>(json);

        gardenName = gardenData.name;
        SerializationController.instance.SetGardenData(StoreData());
    }
}
