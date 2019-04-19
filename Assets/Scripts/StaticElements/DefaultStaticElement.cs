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
        SerializationController.instance.AddToList(this);
    }

    void OnDestroy()
    {
        SerializationController.instance.RemoveFromList(this);
    }

    void OnMouseDrag()
    {
        if (uIController != null)
        {
            MenuScript menu = uIController.GetMenuScript();

            if (menu != null && menu.rotateState)
                menu.RotateGhost();
        }
    }


    //ISelectable
    public override void Select(ConstructionController.ConstructionState state)
    {
        uIController = Camera.main.GetComponent<UIController>();
        if (state == ConstructionController.ConstructionState.Off)
        {
            uIController.SpawnDynMenu(this, uIController.dynamicObjectMenu);
            if (this.data != null)
                uIController.SetDataPanel(this);
        }
    }

    public override void DeSelect()
    {
        if (uIController.GetMenuScript() != null)
            uIController.GetMenuScript().DestroyMenu();
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
