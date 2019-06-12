using System;
using UnityEngine;

public class DefaultStaticElement : GhostHandler
{
    public enum StaticElementType { Wall, Chair, Table };

    public Vector3 correctedRotation;
    public SerializationController.ItemType type;
    public StaticElementType subType;

    protected UIController uIController;

    private void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    protected new void Start()
    {
        gameObject.layer = 0;
        transform.eulerAngles += correctedRotation;
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
    public struct SerializableItemData
    {
        public string type;
        public Vector3 position;
        public Quaternion rotation;
    }

    public override string Serialize()
    {
        SerializableItemData serializableItemData;
        SerializedElement serializedElement;

        serializableItemData.position = transform.position;
        serializableItemData.rotation = transform.rotation;
        serializableItemData.type = subType.ToString();

        serializedElement.type = SerializationController.ItemType.StaticElement;
        serializedElement.data = JsonUtility.ToJson(serializableItemData);
        serializedElement.key = serializationKey;
        return (SerializedElement.ToJson(serializedElement));
    }

    public override void DeSerialize(string json)
    {
        ///serializableItemData = JsonUtility.FromJson<SerializableItem>(json);
        ///transform.position = serializableItemData.position;
        ///transform.rotation = serializableItemData.rotation;
        ///initFromSerialization = true;
    }
}