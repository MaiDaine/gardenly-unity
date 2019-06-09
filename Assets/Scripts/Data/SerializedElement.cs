using System;

[Serializable]
public struct SerializedElement
{
    public SerializationController.ItemType type;
    public int key;
    public string data;

    public static string ToJson(SerializedElement element)
    {
        SimpleJSON.JSONObject json = new SimpleJSON.JSONObject();

        json["type"] = element.type.ToString();
        json["key"] = element.key;
        json["data"] = element.data;

        return (json.ToString());
    }
}