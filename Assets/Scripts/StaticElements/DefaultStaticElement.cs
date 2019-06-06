using System;
using UnityEngine;

public class DefaultStaticElement : GhostHandler, ISerializable
{
    public enum StaticElementType { Chair, Table };

    public Vector3 correctedRotation;
    public SerializationController.ItemType type;
    public StaticElementType subType;

    protected UIController uIController;

    private SerializableItem serializableItem;
    private bool initFromSerialization = false;

    private void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    private void Start()
    {
        if (initFromSerialization)
            initFromSerialization = false;
        else
        {
            transform.eulerAngles += correctedRotation;
            serializableItem.type = subType;
            serializableItem.key = SerializationController.GetCurrentDate();
        }
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
            Camera.main.GetComponent<UIController>().uIInteractions.OnSelectDefaultStaticElement("", "", this);//TODO UI
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
        public int key;
        public StaticElementType type;
        public Vector3 position;
        public Quaternion rotation;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;

        serializableItem.position = transform.position;
        serializableItem.rotation = transform.rotation;

        tmp.type = SerializationController.ItemType.StaticElement;
        tmp.data = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
        transform.position = serializableItem.position;
        transform.rotation = serializableItem.rotation;
        initFromSerialization = true;
    }
}