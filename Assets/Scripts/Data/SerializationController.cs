using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class SerializationController : MonoBehaviour
{
    public enum ItemType { None, StaticElement, FlowerBed, Plant };
    public enum SerializationState { None, Add, Update, Delete };

    public static SerializationController instance = null;

    private GardenData.SerializedGardenData gardenData;
    private List<ISerializable> items;
    private string json = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            items = new List<ISerializable>();
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    public static uint GetElementKey()
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return ((uint)(System.DateTime.UtcNow - epochStart).TotalSeconds);
    }

    public void SetGardenData(GardenData.SerializedGardenData gardenData) { this.gardenData = gardenData; }

    public void AddToList(ISerializable item) { items.Add(item); }

    public void RemoveFromList(ISerializable item) { items.Remove(item); }

    public string Serialize()
    {
        string[] updates = new string[3] { "additions: [", "modifications: [", "deletion: [" };
        bool[] firstEntries = new bool[3] { true, true, true };

        if (items.Count == 0)
            json = "{}";
        else
        {
            json = "{\"name\":\"" + gardenData.name + "\",";
            json += "\"boundaries\":[" + JsonUtility.ToJson(gardenData.boundaries[0]);
            json += ", " + JsonUtility.ToJson(gardenData.boundaries[1]) + "], ";

            foreach (ISerializable item in items)
                switch (item.GetSerializationState())
                {
                    case SerializationState.None:
                        break;
                    case SerializationState.Add:
                        AddItem(ref updates[0], ref firstEntries[0], item);
                        break;
                    case SerializationState.Update:
                        AddItem(ref updates[1], ref firstEntries[1], item);
                        break;
                    case SerializationState.Delete:
                        AddItem(ref updates[2], ref firstEntries[2], item);
                        break;
                }

            for (int i = 0; i < 3; i++)
            {
                if (!firstEntries[i])
                    json += updates[i] + "]";
                if (i + 1 < 3 && !firstEntries[i + 1])
                    json += ",";
            }
            json += '}';
        }
        MessageHandler.instance.SuccesMessage("save_sucessfull");
        ReactProxy.instance.UpdateSaveState(false);
        return json;
    }

    public void DeSerialize(string json)
    {
        SpawnController.instance.loadingData = true;

        var garden = JSON.Parse(json);
        GetComponent<GardenData>().SetGardenName(garden["name"]);

        foreach (var tile in garden["tiles"])
            SpawnController.instance.SpawnFlowerBed(tile.Value.ToString());

        foreach (var plant in garden["plants"])
            SpawnController.instance.SpawnPlantElement(plant.Value.ToString());

        foreach (var staticElement in garden["staticElement"])
        {
            DefaultStaticElement.StaticElementType type = DefaultStaticElement.GetTypeFromString(staticElement.Value["data"]["type"].ToString());
            SpawnController.instance.SpawnStaticElement(staticElement.Value.ToString(), type);
        }

        SpawnController.instance.loadingData = false;
    }

    private void AddItem(ref string modification, ref bool firstEntry, ISerializable item)
    {
        if (!firstEntry)
            modification += ", ";
        else
            firstEntry = false;
        modification += item.Serialize();
    }

    private ItemType GetTypeFromString(string jsonType)
    {
        switch (jsonType)
        {
            case "StaticElement":
                return ItemType.StaticElement;
            case "FlowerBed":
                return ItemType.FlowerBed;
            case "PlantElement":
                return ItemType.Plant;
            default:
                return ItemType.None;
        }
    }
}