using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SerializationController : MonoBehaviour
{
    public enum ItemType { None, DefaultStaticElement, WallHandler, FlowerBed, FlowerBedElement, TreeHandler };

    public static SerializationController instance = null;
    private List<ISerializable> items = new List<ISerializable>();
    private int serializationElemNb;
    private string serializationJSON;

    [DllImport("__Internal")]
    private static extern void SaveScene(string json, int nbElem);

    public void PreInitScene(int nbElem)
    {
        serializationElemNb = nbElem;
    }

    public void InitScene(string json)
    {
        SpawnController.instance.SpawnScene(DeSerialize(json, serializationElemNb));
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            serializationJSON = Serialize(out serializationElemNb);
            SaveScene(serializationJSON, serializationElemNb);
        }
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void AddToList(ISerializable item)
    {
        items.Add(item);
    }

    public void RemoveFromList(ISerializable item)
    {
        items.Remove(item);
    }

    public string Serialize(out int numberElems)
    {
        string result;
        ISerializable[] a = items.ToArray();
        SerializationData[] elems = new SerializationData[a.Length];
        SerializedData serializedData;

        for (int i = 0; i < a.Length; i++)
            elems[i] = a[i].Serialize();
        serializedData.data = elems;
        numberElems = a.Length;
        result = JsonUtility.ToJson(serializedData);
        return (result);
    }

    public SerializationData[] DeSerialize(string json, int itemNumber)
    {
        SerializedData tmp = new SerializedData();
        tmp.data = new SerializationData[itemNumber];
        JsonUtility.FromJsonOverwrite(json, tmp);
        return (tmp.data);
    }
}
