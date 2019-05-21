using System;
using UnityEngine;
using TMPro;

public class DefaultStaticElement : GhostHandler, ISerializable
{
    public enum StaticElementType { Chair, Table, Tree, Tree2 };

    public Vector3 correctedRotation;
    public SerializationController.ItemType type;
    public StaticElementType subType;
    public PlantData plantData;

    protected UIController uIController;

    private SerializableItem serializableItem;

    private void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    private void Start()
    {
        this.transform.eulerAngles += this.correctedRotation;
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
        uIController = Camera.main.GetComponent<UIController>();
        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Off)
        {
            uIController.SpawnDynMenu(this, uIController.dynamicObjectMenu);
            if (this.GetData() != null)
            {
                if (!uIController.PlantsViewsDisplay())
                    uIController.dataPanel.CustomStartAnchoredPosition = new Vector3(122.37f, -33.46f, 0);
                else
                    uIController.dataPanel.CustomStartAnchoredPosition = new Vector3(244.67f, -33.46f, 0);
                if (uIController.dataPanel.GetComponentsInChildren<TextMeshProUGUI>()[0].text != this.data.name || uIController.dataPanel.IsHidden)
                    uIController.SetDataPanel(plantData.name, plantData.type);
            }
        }
    }

    public override void DeSelect()
    {
        MenuScript menuScript = uIController.GetMenuScript();
        if (menuScript != null)
        {
            uIController.GetMenuScript().GetComponentInChildren<LabelScript>().ResetColor();
            menuScript.DestroyMenu();
        }
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
    }

    public void OnPlantDataLoad(PlantData plantData)
    {
        this.plantData = plantData;
    }
}
