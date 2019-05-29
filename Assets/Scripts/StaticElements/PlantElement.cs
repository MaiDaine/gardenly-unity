using System;
using UnityEngine;
using TMPro;

public class PlantElement : GhostHandler, ISelectable, ISerializable
{
    public string plantID;
    public string plantName;
    public string plantType;

    protected SerializedPlantElement serializableItem;


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
        UIController uIController = Camera.main.GetComponent<UIController>();
        TextMeshProUGUI[] labels = uIController.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            RectTransform menuTransform = uIController.extendMenu.RectTransform;
            RectTransform viewTransform = uIController.plantsViews[0].RectTransform;

            uIController.SpawnDynMenu(this);
            uIController.SetDataPanel(plantName, plantType);
        }
    }

    public override void DeSelect()
    {
        if (Camera.main != null)
        {
            UIController uIController = Camera.main.GetComponent<UIController>();

            uIController.Cancel();
        }
    }

    //Serialization
    [Serializable]
    public struct SerializedPlantElement
    {
        public string plantID;
        public Vector3 position;
        public Quaternion rotation;
    }

    public virtual SerializationData Serialize()
    {
        SerializationData tmp = new SerializationData();

        tmp.type = SerializationController.ItemType.PlantElement;
        tmp.data = JsonUtility.ToJson(InnerSerialize());
        return tmp;
    }

    public SerializedPlantElement InnerSerialize()
    {
        SerializedPlantElement tmp;

        tmp.position = transform.position;
        tmp.rotation = transform.rotation;
        tmp.plantID = plantID;
        return (tmp);
    }

    public void InnerDeSerialize(SerializedPlantElement elem)
    {
        plantID = elem.plantID;
        ReactProxy.instance.LoadPlantDataFromId(plantID, OnPlantDataLoad);
        transform.position = elem.position;
        transform.rotation = elem.rotation;
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializedPlantElement>(json);
        transform.position = serializableItem.position;
        transform.rotation = serializableItem.rotation;
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        plantName = plantData.name;
        plantType = plantData.type;
    }
}