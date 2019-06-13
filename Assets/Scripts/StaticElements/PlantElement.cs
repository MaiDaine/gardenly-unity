using System;
using UnityEngine;

public class PlantElement : GhostHandler
{
    private SerializedElement serializedElement;

    public void SetTileKey(uint key) { serializedElement.tile_key = key; }

    protected override void OnEnable()
    {
        base.OnEnable();
        SerializationController.instance.AddToList(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SerializationController.instance.RemoveFromList(this);
    }

    //ISelectable
    public override void Select(ConstructionController.ConstructionState state)
    {
        if (Camera.main != null)
        {
            if (data == null)
                Camera.main.GetComponent<UIController>().uIInteractions.OnSelectPlantElement("", "", this);
            else
                Camera.main.GetComponent<UIController>().uIInteractions.OnSelectPlantElement(data.name, data.typeName, this);
        }
    }

    public override void DeSelect()
    {
        if (Camera.main != null)
            Camera.main.GetComponent<UIController>().uIInteractions.OnDeSelectPlantElement();
    }

    //Serialization
    [Serializable]
    public struct SerializedElement
    {
        public SerializationController.ItemType type;
        public int key;
        public string plant_id;
        public uint tile_key;
        public int age;
        public float sun_exposition;
        public string data;
    }

    [Serializable]
    public struct SerializableItemData
    {
        public Vector3 position;
    }

    public override string Serialize()
    {
        SerializableItemData serializableItemData;

        serializableItemData.position = transform.position;
        
        serializedElement.type = SerializationController.ItemType.Plant;
        serializedElement.data = JsonUtility.ToJson(serializableItemData);

        SimpleJSON.JSONObject json = new SimpleJSON.JSONObject();

        json["type"] = serializedElement.type.ToString();
        json["key"] = serializedElement.key;
        json["plant_id"] = serializedElement.plant_id;
        json["tile_key"] = serializedElement.tile_key;
        json["age"] = "0";
        json["sun_exposition"] = "0.0";
        json["data"] = serializedElement.data;

        return (json.ToString());
    }

    public override void DeSerialize(string json)
    {
        //serializableItem = JsonUtility.FromJson<SerializableItemData>(json);
        //data.plantID = serializableItem.plant_id;
        //transform.position = serializableItem.position;
        ////age
        ////sun
        //ReactProxy.instance.LoadPlantDataFromId(data.plantID, OnPlantDataLoad);
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        data = plantData;//TODO
        serializedElement.plant_id = plantData.plantID;
        // GetComponent<MeshRenderer>().material = SpawnController.instance.GetModelMaterial(plantData);
    }
}