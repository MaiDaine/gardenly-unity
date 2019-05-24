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

    void Start()
    {
        this.transform.eulerAngles += this.correctedRotation;
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
            uIController.SpawnDynMenu(this);
            if (!uIController.PlantsViewsDisplay())
                uIController.dataPanel.GetView().CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x + 0.3f, -33.46f, 0);
            else
                uIController.dataPanel.GetView().CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x + viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);
            if (labels[labels.Length - 1].text != plantName || uIController.dataPanel.GetView().IsHidden)
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

        tmp.position = this.transform.position;
        tmp.rotation = this.transform.rotation;
        tmp.subID = subID;
        return (tmp);
    }

    public void InnerDeSerialize(SerializedFBE elem)
    {
        this.subID = elem.subID;
        ReactProxy.instance.LoadPlantDataFromId(this.subID, OnPlantDataLoad);
        this.transform.position = elem.position;
        this.transform.rotation = elem.rotation;
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializedFBE>(json);
        this.transform.position = serializableItem.position;
        this.transform.rotation = serializableItem.rotation;
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        plantName = plantData.name;
        plantType = plantData.type;
    }
}
    
