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

    void Start()
    {
        uIController = Camera.main.GetComponent<UIController>();
        this.transform.eulerAngles += correctedRotation;
        SerializationController.instance.AddToList(this);
    }

    void OnDestroy()
    {
        SerializationController.instance.RemoveFromList(this);
    }

    void OnMouseDrag()
    {
        MenuScript menu = uIController.GetMenuScript();

        if (menu != null && menu.rotateState)
            menu.RotateGhost();
    }


    //ISELECTABLE
    public override void Select(ConstructionController.ConstructionState state)
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        if (state == ConstructionController.ConstructionState.Off)
                uIController.SpawnDynMenu(this, uIController.dynamicObjectMenu);
    }


    //Serialization
    [Serializable]
    public struct SerializableItem
    {
        public StaticElementType subType;
        public Transform transform;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;

        serializableItem.transform = this.transform;
        serializableItem.subType = subType;
        tmp.type = SerializationController.ItemType.DefaultStaticElement;
        tmp.serializedData = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        this.transform.position = serializableItem.transform.position;
        this.transform.rotation = serializableItem.transform.rotation;
        this.transform.localScale = serializableItem.transform.localScale;
    }
}
