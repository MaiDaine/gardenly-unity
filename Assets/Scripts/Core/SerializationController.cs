using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationController : MonoBehaviour
{
    public enum ItemType { None, DefaultStaticElement, WallHandler };

    public static SerializationController instance = null;
    private List<ISerializable> items = new List<ISerializable>();

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

    public string Serialize()
    {
        ISerializable[] a = items.ToArray();
        SerializationData[] elems = new SerializationData[a.Length];
        SerializedData serializedData;

        for (int i = 0; i < a.Length; i++)
            elems[i] = a[i].Serialize();
        serializedData.data = elems;
        return(JsonUtility.ToJson(serializedData));
    }

    public SerializationData[] DeSerialize(string json, int itemNumber)
    {
        SerializedData tmp = new SerializedData();
        tmp.data = new SerializationData[itemNumber];
        JsonUtility.FromJsonOverwrite(json, tmp);
        return (tmp.data);
    }
}
