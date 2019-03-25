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
            SpawnController.instance.SpawnScene(DeSerialize("{\"garden\":[{\"type\":\"FlowerBed\",\"data\":{\"name\":\"FlowerBed\",\"pointsSize\":5,\"points\":[{\"x\":46.310054779052734,\"y\":-69.38301086425781},{\"x\":48.709495544433594,\"y\":-64.33088684082031},{\"x\":53.19804000854492,\"y\":-65.67674255371094},{\"x\":50.14833068847656,\"y\":-68.9350357055664},{\"x\":46.310054779052734,\"y\":-69.38301086425781}],\"elementsSize\":3,\"elements\":[{\"subID\":0,\"position\":{\"x\":49.71393585205078,\"y\":9.5367431640625e-7,\"z\":-65.6791763305664},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":0,\"position\":{\"x\":49.534976959228516,\"y\":0,\"z\":-67.25696563720703},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}},{\"subID\":0,\"position\":{\"x\":50.65749740600586,\"y\":-9.5367431640625e-7,\"z\":-66.22111511230469},\"rotation\":{\"x\":0,\"y\":0,\"z\":0,\"w\":1}}]}},{\"type\":\"WallHandler\",\"data\":{\"start\":{\"x\":44.7247200012207,\"y\":-9.5367431640625e-7,\"z\":-67.14225769042969},\"end\":{\"x\":48.633277893066406,\"y\":-9.5367431640625e-7,\"z\":-62.90876770019531}}},{\"type\":\"WallHandler\",\"data\":{\"start\":{\"x\":50.23482894897461,\"y\":-9.5367431640625e-7,\"z\":-63.20880889892578},\"end\":{\"x\":56.12355041503906,\"y\":9.5367431640625e-7,\"z\":-64.91214752197266}}},{\"type\":\"WallHandler\",\"data\":{\"start\":{\"x\":44.066707611083984,\"y\":0,\"z\":-69.7416763305664},\"end\":{\"x\":50.33789825439453,\"y\":0,\"z\":-71.0821762084961}}},{\"type\":\"WallHandler\",\"data\":{\"start\":{\"x\":51.573123931884766,\"y\":0,\"z\":-69.9146957397461},\"end\":{\"x\":55.243289947509766,\"y\":0,\"z\":-65.70992279052734}}}]}"));
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
