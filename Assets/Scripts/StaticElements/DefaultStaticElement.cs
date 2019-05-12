using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStaticElement : GhostHandler, ISerializable
{
    public enum StaticElementType { Chair, Table, Tree, Tree2 };

    public Vector3 correctedRotation;
    public SerializationController.ItemType type;
    public StaticElementType subType;

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
        if (state == ConstructionController.ConstructionState.Off)
        {
            uIController.SpawnDynMenu(this, uIController.dynamicObjectMenu);
            if (this.GetData() != null)
            {
                //uIController.SetDataPanel(this);
                uIController.gardenMenu.gameObject.SetActive(true);
            }
        }
    }

    public override void DeSelect()
    {
        MenuScript menuScript = uIController.GetMenuScript();
        if (menuScript != null)
        {
            menuScript.DestroyMenu();
            uIController.dataPanel.gameObject.SetActive(false);
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
}
