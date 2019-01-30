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
    public Transform windowPreview;

    private SerializableItem serializableItem;
    protected Transform previewUI;
    protected MenuScript menu;

    void Start()
    {
        this.transform.eulerAngles += correctedRotation;
        SerializationController.instance.AddToList(this);
        previewUI = null;
    }

    void OnDestroy()
    {
        SerializationController.instance.RemoveFromList(this);
    }

    void SpawnWindow()
    {
        Canvas canvas;
        Vector3 position;

        position = new Vector3(transform.position.x, transform.position.y + 6, transform.position.z);
        
        previewUI = Instantiate(windowPreview, position, Quaternion.identity);
        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        menu = previewUI.GetComponent<MenuScript>();
        menu.SetGhostRef(this);
    }

    void OnMouseDrag()
    {
        if (menu != null && menu.rotateState)
            menu.RotateGhost();
    }


    //ISELECTABLE
    public override void Select(ConstructionController.ConstructionState state)
    {
        if (previewUI == null)
            SpawnWindow();
        else if (!previewUI.gameObject.activeSelf)
            previewUI.gameObject.SetActive(true);
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
        tmp.type = SerializationController.ItemType.WallHandler;
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
