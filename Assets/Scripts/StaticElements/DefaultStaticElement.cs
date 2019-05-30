using System;
using UnityEngine;
using TMPro;

public class DefaultStaticElement : GhostHandler, ISerializable
{
    public enum StaticElementType { Chair, Table };

    public Vector3 correctedRotation;
    public SerializationController.ItemType type;
    public StaticElementType subType;
    public string plantName;
    public string plantType;

    protected UIController uIController;

    private SerializableItem serializableItem;
    private bool initFromSerialization = false;

    private void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    private void Start()
    {
        if (!initFromSerialization)
            this.transform.eulerAngles += this.correctedRotation;
        else
            initFromSerialization = false;
    }

    void OnDestroy()
    {
    }

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
            Camera.main.GetComponent<UIController>().uIInteractions.OnSelectDefaultStaticElement(plantName, plantType, this);
    }

    public override void DeSelect()
    {
        if (Camera.main != null)
            Camera.main.GetComponent<UIController>().uIInteractions.OnDeselectDefaultStaticElement();
    }


    //Serialization
    [Serializable]
    public struct SerializableItem
    {
        public StaticElementType subType;
        public Vector3 position;
        public Quaternion rotation;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;

        this.serializableItem.position = this.transform.position;
        this.serializableItem.rotation = this.transform.rotation;
        this.serializableItem.subType = this.subType;
        tmp.type = SerializationController.ItemType.DefaultStaticElement;
        tmp.data = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        this.serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        this.transform.position = this.serializableItem.position;
        this.transform.rotation = this.serializableItem.rotation;
        initFromSerialization = true;
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        plantName = plantData.name;
        plantType = plantData.type;
    }
}
