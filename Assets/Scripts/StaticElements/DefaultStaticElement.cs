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

    void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    void Start()
    {
        this.transform.eulerAngles += correctedRotation;
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


    //ISELECTABLE
    public override void Select(ConstructionController.ConstructionState state)
    {
        if (state == ConstructionController.ConstructionState.Off)
                uIController.SpawnDynMenu(this, uIController.dynamicObjectMenu);
    }

    public override void DeSelect()
    {
        if (uIController.GetMenuScript() != null && uIController.GetMenuScript().rotateState)
        {
            uIController.GetMenuScript().rotateState = false;
            uIController.GetMenuScript().GetComponentInChildren<LabelScript>().ResetColor();
        }
        // TODO si le menu bloque le ray cast appel destroymenu
        //uIController.GetMenuScript().DestroyMenu();
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

        serializableItem.position = this.transform.position;
        serializableItem.rotation = this.transform.rotation;
        serializableItem.subType = subType;
        tmp.type = SerializationController.ItemType.DefaultStaticElement;
        tmp.data = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        this.transform.position = serializableItem.position;
        this.transform.rotation = serializableItem.rotation;
    }
}
