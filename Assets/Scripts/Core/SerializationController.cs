using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using SimpleJSON;

public class SerializationController : MonoBehaviour
{
    public enum ItemType { None, GardenData, DefaultStaticElement, WallHandler, FlowerBed, FlowerBedElement, TreeHandler };

    public static SerializationController instance = null;

    private GardenData.SerializedGardenData gardenData;
    private int serializationElemNb = 0;
    private List<ISerializable> items = new List<ISerializable>();
    private string json = "";
    private bool closed = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void SetGardenData(GardenData.SerializedGardenData gardenData) { this.gardenData = gardenData; }

    public void AddToList(ISerializable item) { this.items.Add(item); }

    public void RemoveFromList(ISerializable item) { this.items.Remove(item); }

    public void Serialize()
    {
        if (this.items.Count == 0)
        {
            this.json = "{}";
            this.closed = true;
        }
        else
        {
            this.json = "{\"name\":\"" + gardenData.name + "\",";
            this.json += "\"boundaries\":[" + JsonUtility.ToJson(gardenData.boundaries[0]);
            this.json += ", " + JsonUtility.ToJson(gardenData.boundaries[1]) + "],";
            this.serializationElemNb = 0;
            this.closed = false;
            foreach (ISerializable item in items)
                AddItem(CreateItem(item.Serialize()));
        }
        ErrorHandler.instance.SuccesMessage("Save sucessfull");
        ReactProxy.instance.UpdateSaveState(false);
    }

    public string GetSerializedData()
    {
        if (!this.closed && json != "")
        {
            this.json += "]}";
            this.closed = true;
        }
        return this.json;
    }

    public SerializationData[] DeSerialize(string json)
    {
        var tmp = JSON.Parse(json);
        SerializedData serializedData = new SerializedData();
        int lenght = tmp["garden"].AsArray.Count;
        serializedData.data = new SerializationData[lenght];
        for (int i = 0; i < lenght; i++)
        {
            SerializationData elem = new SerializationData();
            elem.type = GetTypeFromString(tmp["garden"][i]["type"]);
            elem.data = tmp["garden"][i]["data"].ToString();
            serializedData.data[i] = elem;
        }
        return (serializedData.data);
    }


    private ItemType GetTypeFromString(string jsonType)
    {
        switch (jsonType)
        {
            case "DefaultStaticElement":
                return ItemType.DefaultStaticElement;
            case "WallHandler":
                return ItemType.WallHandler;
            case "TreeHandler":
                return ItemType.TreeHandler;
            case "FlowerBed":
                return ItemType.FlowerBed;
            case "FlowerBedElement":
                return ItemType.FlowerBedElement;
            default:
                return ItemType.None;
        }
    }

    private void AddItem(string item)
    {
        if (this.serializationElemNb == 0)
            this.json += "\"garden\":[" + item;
        else
            this.json += "," + item;
        this.serializationElemNb++;
    }

    private string CreateItem(SerializationData item)
    {
        string tmp = "{\"type\":\"" + item.type.ToString() + "\"," + "\"data\":" + item.data + "}";
        return tmp;
    }
}
