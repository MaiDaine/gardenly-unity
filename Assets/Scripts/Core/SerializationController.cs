using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using SimpleJSON;

public class SerializationController : MonoBehaviour
{
    public enum ItemType { None, DefaultStaticElement, WallHandler, FlowerBed, FlowerBedElement, TreeHandler };

    public static SerializationController instance = null;
    private List<ISerializable> items = new List<ISerializable>();
    public int serializationElemNb = 0;
    public string serializationJSON;
    private string json = "";
    private bool closed = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))//TODO DEBUG ONLY
        {
            Serialize();
            Debug.Log(GetSerializedData());
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
            SpawnController.instance.SpawnScene(DeSerialize("{\"garden\":[{\"type\":\"FlowerBed\",\"data\":{\"meshPosition\":{\"x\":0,\"y\":0,\"z\":0},\"meshNumber\":2,\"pointsList\":{\"positions\":[{\"x\":-5.512813091278076,\"y\":9.5367431640625e-7,\"z\":-7.512477397918701},{\"x\":-3.5871775150299072,\"y\":0,\"z\":-7.471221923828125}],\"points\":[{\"x\":1,\"y\":-1},{\"x\":-1,\"y\":-1},{\"x\":-1,\"y\":1.0000004768371582},{\"x\":1,\"y\":1.0000004768371582},{\"x\":0.9999997615814209,\"y\":-1},{\"x\":-1,\"y\":-1},{\"x\":-1,\"y\":1},{\"x\":0.9999997615814209,\"y\":1}]},\"FBEElements\":[{\"subType\":0,\"position\":{\"x\":-5.663893222808838,\"y\":0,\"z\":-7.510650634765625},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subType\":0,\"position\":{\"x\":-4.4959797859191895,\"y\":9.5367431640625e-7,\"z\":-7.460208892822266},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subType\":0,\"position\":{\"x\":-3.2236928939819336,\"y\":0,\"z\":-7.475632667541504},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}}]}}]}"));
    }

    public void AddToList(ISerializable item) { items.Add(item); }

    public void RemoveFromList(ISerializable item) { items.Remove(item); }

    public void Serialize()
    {
        if (items.Count == 0)
        {
            ErrorHandler.instance.ErrorMessage("Nothing to save");
            return;
        }
        json = "";
        serializationElemNb = 0;
        closed = false;
        foreach (ISerializable item in items)
            AddItem(CreateItem(item.Serialize()));
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

    public string GetSerializedData()
    {
        if (!closed)
        {
            json += "]}";
            closed = true;
        }
        return json;
    }

    private void AddItem(string item)
    {
        if (serializationElemNb == 0)
            json = "{\"garden\":[" + item;
        else
            json += "," + item;
        serializationElemNb++;
    }

    private string CreateItem(SerializationData item)
    {
        string tmp = "{\"type\":\"" + item.type.ToString() + "\"," + "\"data\":" + item.data + "}";
        return tmp;
    }
}
