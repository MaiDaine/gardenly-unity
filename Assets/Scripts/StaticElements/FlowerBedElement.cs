using System;
using UnityEngine;
using TMPro;

public class FlowerBedElement : GhostHandler, ISelectable, ISerializable
{
    public SerializationController.ItemType type;
    public string subID;
    public string plantName;
    public string plantType;
    public Vector3 correctedRotation;

    private SerializedFBE serializableItem;

    private void Start()
    {
        transform.eulerAngles += correctedRotation;
        if (ConstructionController.instance.flowerBeds.Count < 1)
        {
            MessageHandler.instance.ErrorMessage("flower_bed", "no_flowerbed");
            ConstructionController.instance.Cancel();
        }
    }

    public override void Select(ConstructionController.ConstructionState state)
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        TextMeshProUGUI[] labels = uIController.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            RectTransform menuTransform = uIController.extendMenu.RectTransform;
            RectTransform viewTransform = uIController.plantsViews[0].RectTransform;
            uIController.SpawnDynMenu(this, uIController.dynamicObjectMenu);
            if (!uIController.PlantsViewsDisplay())
                uIController.dataPanel.CustomStartAnchoredPosition = new Vector3(-menuTransform.sizeDelta.x + 0.3f, -33.46f, 0);
            else
                uIController.dataPanel.CustomStartAnchoredPosition = new Vector3(-menuTransform.sizeDelta.x + viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);
            if (labels[labels.Length - 1].text != plantName || uIController.dataPanel.IsHidden)
                uIController.SetDataPanel(plantName, plantType);
        }
    }

    public override void DeSelect()
    {
        if (Camera.main != null)
        {
            UIController uIController = Camera.main.GetComponent<UIController>();

            if (uIController.GetMenuScript() != null)
            {
                uIController.GetMenuScript().GetComponentInChildren<LabelScript>().ResetColor();
                uIController.GetMenuScript().DestroyMenu();
            }
            else
                uIController.DestroyMenu();
        }
    }

    //Serialization
    [Serializable]
    public struct SerializedFBE
    {
        public string subID;
        public Vector3 position;
        public Quaternion rotation;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp = new SerializationData();

        tmp.type = SerializationController.ItemType.FlowerBedElement;
        tmp.data = JsonUtility.ToJson(InnerSerialize());
        return tmp;
    }

    public SerializedFBE InnerSerialize()
    {
        SerializedFBE tmp;

        tmp.position = transform.position;
        tmp.rotation = transform.rotation;
        tmp.subID = subID;
        return (tmp);
    }

    public void InnerDeSerialize(SerializedFBE elem)
    {
        subID = elem.subID;
        ReactProxy.instance.LoadPlantDataFromId(subID, OnPlantDataLoad);
        transform.position = elem.position;
        transform.rotation = elem.rotation;
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializedFBE>(json);
        transform.position = serializableItem.position;
        transform.rotation = serializableItem.rotation;
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        plantName = plantData.name;
        plantType = plantData.type;
    }
}