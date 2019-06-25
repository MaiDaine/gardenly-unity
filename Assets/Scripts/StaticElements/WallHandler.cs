using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Material))]
public class WallHandler : GhostHandler, ISerializable
{
    public LineTextHandler textRef;

    protected UIController uIController;

    private LineTextHandler text = null;
    private Vector3 start;
    private Vector3 end;

    private void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }

    private new void Start()
    {
        if (!initFromSerialization)
        {
            gameObject.layer = 0;
            transform.localScale = new Vector3(0.1f, 3f, 0.1f);
        }
    }

    private void OnDestroy()
    {
        if (text != null)
            GameObject.Destroy(text);
    }

    //public override void Positioning(Vector3 position) { base.Positioning(position); }

    public override bool FromPositioningToBuilding(Vector3 position)
    {
        if (text == null)
            text = Instantiate(textRef, transform.position, Quaternion.identity) as LineTextHandler;
        else
            text.transform.position = position;
        text.gameObject.SetActive(true);
        if (!(uIController.GetMenuScript() != null && uIController.GetMenuScript().isMoving))
            start = position;
        return true;
    }

    public override bool Building(Vector3 position)
    {
        //if (uIController.GetMenuScript() != null && uIController.GetMenuScript().isMoving)
        //{
        //    start += (transform.position - position);
        //    end += (transform.position - position);
        //    transform.position = position;
        //    text.transform.position = position;
        //    return false;
        //}
        if (start != position && end != position)
        {
            end = position;
            Vector3 tmp = (start + end) / 2f;
            float lenght = (start - end).magnitude;

            transform.position = tmp;
            transform.rotation = (Quaternion.LookRotation(end - start, Vector3.up) * Quaternion.Euler(0, 90, 0));
            transform.localScale = new Vector3(lenght, transform.localScale.y, transform.localScale.z);

            text.transform.position = tmp;
            text.SetText(string.Format("{0:F1}m", lenght));
        }
        return false;
    }

    public override void EndConstruction(Vector3 position)
    {
        base.EndConstruction(position);
        text.gameObject.SetActive(false);
    }

    public override void Move(Vector3 position)
    {
        start += (position - transform.position);
        end += (position - transform.position);
        base.Move(position);
        text.transform.position = position;
    }

    //ISelectable
    public override void Select(ConstructionController.ConstructionState state)
    {
        if (Camera.main != null)
            Camera.main.GetComponent<UIController>().uIInteractions.OnSelectWall(this, state);
    }

    public override List<ISelectable> SelectWithNeighbor()
    {
        Select(ConstructionController.ConstructionState.Off);
        List<ISelectable> tmp = new List<ISelectable>(neighbors);
        foreach (ISelectable item in tmp)
            item.Select(ConstructionController.ConstructionState.Off);
        return tmp;
    }

    public override void DeSelect()
    {
        if (Camera.main != null)
            Camera.main.GetComponent<UIController>().uIInteractions.OnDeselectWall(text);
    }

    protected override void OnEnable()
    {
        if (text != null)
            PlayerController.instance.SelectFromAction(GetComponent<ISelectable>());
        SerializationController.instance.AddToList(this);
    }

    protected override void OnDisable()
    {
        if (text != null)
            GameObject.Destroy(text);
        base.OnDisable();
    }

    //ISnapable
    public override bool FindSnapPoint(ref Vector3 currentPos, float snapDistance)
    {
        if (((start - currentPos).sqrMagnitude < (end - currentPos).sqrMagnitude)
            && ((start - currentPos).magnitude < snapDistance))
        {
            currentPos = start;
            return true;
        }
        else if ((end - currentPos).magnitude < snapDistance)
        {
            currentPos = end;
            return true;
        }
        return false;
    }

    //Tools
    public LineTextHandler GetText() { return text; }

    //Serialization
    [Serializable]
    public struct SerializableItemData
    {
        public string type;
        public Vector3 start;
        public Vector3 end;
        public Quaternion rotation;
    }

    public override string Serialize()
    {
        SerializableItemData serializableItemData;
        SerializedElement serializedElement;

        serializableItemData.type = "Wall";
        serializableItemData.start = start;
        serializableItemData.end = end;
        serializableItemData.rotation = transform.rotation;

        serializedElement.type = SerializationController.ItemType.StaticElement;
        serializedElement.data = JsonUtility.ToJson(serializableItemData);
        serializedElement.key = serializationKey;
        return (SerializedElement.ToJson(serializedElement));
    }

    public override void DeSerialize(string json)
    {
        SerializedElement serializedElement = JsonUtility.FromJson<SerializedElement>(json);
        SerializableItemData serializableItemData = JsonUtility.FromJson<SerializableItemData>(serializedElement.data);

        serializationKey = serializedElement.key;
        initFromSerialization = true;

        Positioning(serializableItemData.start);
        FromPositioningToBuilding(serializableItemData.start);
        Building(serializableItemData.end);
        EndConstruction(serializableItemData.end);
        initFromSerialization = true;
        transform.rotation = serializableItemData.rotation;
    }
}