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

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))//TODO DEBUG ONLY
        {
            Serialize();
            Debug.Log(GetSerializedData());
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
            SpawnController.instance.SpawnScene(DeSerialize("{\"name\":\"PlaceHolder\",\"boundaries\":[{\"x\":0,\"y\":0},{\"x\":100,\"y\":100}],\"garden\":[{\"type\":\"FlowerBed\",\"data\":{\"name\":\"FlowerBed\",\"points\":[{\"x\":46.46209716796875,\"y\":-66.8306655883789},{\"x\":51.839595794677734,\"y\":-59.973602294921875},{\"x\":59.73530578613281,\"y\":-67.19570922851562},{\"x\":56.854225158691406,\"y\":-69.28050231933594},{\"x\":46.46209716796875,\"y\":-66.8306655883789}],\"elements\":[{\"subID\":2,\"position\":{\"x\":50.28996658325195,\"y\":-9.5367431640625e-7,\"z\":-65.2952651977539},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":1,\"position\":{\"x\":51,\"y\":0,\"z\":-64},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":0,\"position\":{\"x\":52.146240234375,\"y\":0,\"z\":-65.69538879394531},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":0,\"position\":{\"x\":53.64667510986328,\"y\":0,\"z\":-64.77925872802734},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":1,\"position\":{\"x\":54.98756790161133,\"y\":0,\"z\":-65.5239486694336},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":0,\"position\":{\"x\":53.67853927612305,\"y\":0,\"z\":-66.25352478027344},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}}]}},{\"type\":\"DefaultStaticElement\",\"data\":{\"subType\":0,\"position\":{\"x\":43.46509552001953,\"y\":0,\"z\":-62.66104507446289},\"rotation\":{\"x\":1,\"y\":0,\"z\":0,\"w\":-4.371138828673793e-8}}},{\"type\":\"WallHandler\",\"data\":{\"start\":{\"x\":43.43412780761719,\"y\":0,\"z\":-59.35613250732422},\"end\":{\"x\":47.7480354309082,\"y\":9.5367431640625e-7,\"z\":-57.53746032714844}}},{\"type\":\"WallHandler\",\"data\":{\"start\":{\"x\":43.254878997802734,\"y\":-9.5367431640625e-7,\"z\":-65.11761474609375},\"end\":{\"x\":44.40459442138672,\"y\":-9.5367431640625e-7,\"z\":-69.12673950195312}}}]}"));
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Vector2 tmp = new Vector2(1, 1);
            Debug.Log(JsonUtility.ToJson(tmp));
        }
    }

    public void SetGardenData(GardenData.SerializedGardenData gardenData) { this.gardenData = gardenData; }

    public void AddToList(ISerializable item) { this.items.Add(item); }

    public void RemoveFromList(ISerializable item) { this.items.Remove(item); }

    public void Serialize()
    {
        if (this.items.Count == 0)
        {
            ErrorHandler.instance.ErrorMessage("Nothing to save");
            return;
        }
        this.json = "{\"name\":\"" + gardenData.name + "\",";
        this.json += "\"boundaries\":[" + JsonUtility.ToJson(gardenData.boundaries[0]);
        this.json += ", " + JsonUtility.ToJson(gardenData.boundaries[1]) + "],";
        this.serializationElemNb = 0;
        this.closed = false;
        foreach (ISerializable item in items)
            AddItem(CreateItem(item.Serialize()));

        // CALL FOR SUCCESS MSG ErrorHandler.instance.SuccesMessage("The scene has been saved");
    }

    public string GetSerializedData()
    {
        if (!this.closed)
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
