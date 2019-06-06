using System;
using UnityEngine;

public class PlantElement : GhostHandler, ISelectable, ISerializable
{
    protected SerializableItem serializableItem;

    private bool initFromSerialization = false;

    private void Start()
    {
        if (ConstructionController.instance.flowerBeds.Count < 1)
        {
            MessageHandler.instance.ErrorMessage("flower_bed", "no_flowerbed");
            ConstructionController.instance.Cancel();
        }
        if (initFromSerialization)
            initFromSerialization = false;
        else
            serializableItem.key = SerializationController.GetCurrentDate();
    }

    public void SetTileKey(int key) { serializableItem.tile_key = key; }

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
            Camera.main.GetComponent<UIController>().uIInteractions.OnSelectPlantElement(data.name, data.type, this);
    }

    public override void DeSelect()
    {
        if (Camera.main != null)
            Camera.main.GetComponent<UIController>().uIInteractions.OnDeSelectPlantElement();
    }

    //Serialization
    [Serializable]
    public struct SerializableItem
    {
        public int key;
        public string plant_id;
        public int tile_key;
        public Vector3 position;
        public int age;
        public float sun_exposition;
    }

    public virtual SerializationData Serialize()
    {
        SerializationData tmp;

        serializableItem.plant_id = data.plantID;
        serializableItem.position = transform.position;
        serializableItem.age = 0;
        serializableItem.sun_exposition = 0f;

        tmp.type = SerializationController.ItemType.PlantElement;
        tmp.data = JsonUtility.ToJson(serializableItem);
        return tmp;
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        data.plantID = serializableItem.plant_id;
        transform.position = serializableItem.position;
        //age
        //sun
        ReactProxy.instance.LoadPlantDataFromId(data.plantID, OnPlantDataLoad);
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        data = plantData;
        // GetComponent<MeshRenderer>().material = SpawnController.instance.GetModelMaterial(plantData);
    }
}