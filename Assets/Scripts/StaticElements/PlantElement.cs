using SimpleJSON;
using System;
using UnityEngine;

public class PlantElement : GhostHandler
{
    private SerializedElement serializedElement;
    private SerializableItemData serializableItemData;

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

    public void UpdateSunExposure()
    {
        serializedElement.sun_exposition = ShadowMap.instance.GetSunExposure(transform.position.x, transform.position.z);
    }

    //ISelectable
    public override void Select(ConstructionController.ConstructionState state)
    {
        base.Select(state);
        if (Camera.main != null)
        {
            UpdateSunExposure();
            if (data == null)
                Camera.main.GetComponent<UIController>().uIInteractions.OnSelectPlantElement("", "", this);
            else
                Camera.main.GetComponent<UIController>().uIInteractions.OnSelectPlantElement(data.name, data.typeName, this);
        }
    }

    public override void DeSelect()
    {
        base.DeSelect();
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
        public string plantType;
        public string plantName;
        public int model;
    }

    public override string Serialize()
    {
        serializedElement.type = SerializationController.ItemType.Plant;
        serializableItemData.position = transform.position;
        serializedElement.data = JsonUtility.ToJson(serializableItemData);

        SimpleJSON.JSONObject json = new SimpleJSON.JSONObject();

        json["type"] = serializedElement.type.ToString();
        json["key"] = serializationKey.ToString();
        json["plant_id"] = serializedElement.plant_id;
        json["tile_key"] = serializedElement.tile_key.ToString();
        json["age"] = "0";
        json["sun_exposition"] = serializedElement.sun_exposition;
        json["data"] = serializedElement.data;

        return (json.ToString());
    }

    public override void DeSerialize(string json)
    {
        SerializedElement serializedElement = JsonUtility.FromJson<SerializedElement>(json);
        serializableItemData = JsonUtility.FromJson<SerializableItemData>(serializedElement.data);

        var tmp = JSON.Parse(json);
        data = new PlantData(tmp["plant"]["name"]);
        data.plantID = tmp["plant"]["id"];

        string fbID = tmp["tile"]["id"];
        foreach (FlowerBed fb in ConstructionController.instance.flowerBeds)
            if (fb.dbID == fbID)
            {
                serializedElement.tile_key = fb.GetKey();
                fb.AddElement(this);
            }

        transform.position = serializableItemData.position;
        if (serializableItemData.model > 0 && serializableItemData.model < ReactProxy.instance.modelList.Models.Count)
            SetModel(ReactProxy.instance.modelList.Models[serializableItemData.model]);
        //age
        //sun
        if (!Application.isEditor)
            ReactProxy.instance.LoadPlantDataFromSave(OnPlantDataLoad, data.plantID, serializableItemData.plantName, serializableItemData.plantType);
        if (serializableItemData.model > 0 && serializableItemData.model < SpawnController.instance.modelList.Models.Count)
            SetModel(SpawnController.instance.modelList.Models[serializableItemData.model]);
    }

    public void SetModel(int model) { serializableItemData.model = model; }

    public void OnPlantDataLoad(PlantData plantData, GameObject model = null)
    {
        data = plantData;
        serializedElement.plant_id = plantData.plantID;
        serializableItemData.plantType = plantData.typeName;
        serializableItemData.plantName = plantData.name;
        if (model != null)
            SetModel(model);
    }

    private void SetModel(GameObject model)
    {
        if (model.GetComponent<MeshFilter>() == null)
            GetComponent<MeshFilter>().mesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;
        else
            GetComponent<MeshFilter>().mesh = model.GetComponent<MeshFilter>().sharedMesh;
        if (model.GetComponent<MeshRenderer>() == null)
            GetComponent<MeshRenderer>().materials = model.GetComponentInChildren<MeshRenderer>().sharedMaterials;
        else
            GetComponent<MeshRenderer>().materials = model.GetComponent<MeshRenderer>().sharedMaterials;
        DestroyImmediate(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }
}