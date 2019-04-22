using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Material))]
public class WallHandler : GhostHandler, ISerializable
{
    public WallTextHandler TextRef;

    protected UIController uIController;

    private SerializableItem serializableItem;
    private WallTextHandler text = null;
    private Vector3 start;
    private Vector3 end;
    private bool initFromSerialization = false;

    private void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }
    
    private void Start()
    {
        if (initFromSerialization)
        {
            Positioning(this.serializableItem.start);
            FromPositioningToBuilding(this.serializableItem.start);
            Building(this.serializableItem.end);
            EndConstruction(this.serializableItem.end);
        }
        else
        {
            this.gameObject.layer = 0;
            this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
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
        if (this.text == null)
            this.text = Instantiate(this.TextRef, this.transform.position, Quaternion.identity) as WallTextHandler;
        else
            this.text.transform.position = position;
        this.text.gameObject.SetActive(true);
        if (!(this.uIController.GetMenuScript() != null && this.uIController.GetMenuScript().isMoving))
            this.start = position;
        return true;
    }

    public override bool Building(Vector3 position)
    {
        if (uIController.GetMenuScript() != null && uIController.GetMenuScript().isMoving)
        {
            start += (this.transform.position - position);
            end += (this.transform.position - position);
            this.transform.position = position;
            text.transform.position = position;
            return false;
        }
        if (start != position && end != position)
        {
            end = position;
            Vector3 tmp = (start + end) / 2f;
            float lenght = (start - end).magnitude;

            this.transform.position = tmp;
            this.transform.rotation = (Quaternion.LookRotation(end - start, Vector3.up) * Quaternion.Euler(0, 90, 0));
            this.transform.localScale = new Vector3(lenght, transform.localScale.y, transform.localScale.z);

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
        base.Move(position);
        this.start += (this.transform.position - position);
        this.end += (this.transform.position - position);
        this.text.transform.position = position;
    }

    //ISelectable
    public override void Select(ConstructionController.ConstructionState state)
    {
        if (state == ConstructionController.ConstructionState.Off)
            this.uIController.SpawnDynMenu(this, this.uIController.wallMenu);

        this.text.gameObject.SetActive(true);
    }

    public override List<ISelectable> SelectWithNeighbor()
    {
        Select(ConstructionController.ConstructionState.Off);
        List<ISelectable> tmp = new List<ISelectable>(this.neighbors);
        foreach (ISelectable item in tmp)
            item.Select(ConstructionController.ConstructionState.Off);
        return tmp;
    }

    public override void DeSelect()
    {
        MenuScript menuScript = this.uIController.GetMenuScript();

        if (this.text.gameObject != null)
            this.text.gameObject.SetActive(false);

        if (menuScript != null)
            menuScript.DestroyMenu();
    }

    protected override void OnEnable()
    {
        if (text != null)
            PlayerController.instance.SelectFromAction(this.GetComponent<ISelectable>());
        SerializationController.instance.AddToList(this);
    }

    protected override void OnDisable()
    {
        SerializationController.instance.RemoveFromList(this);
    }

    //ISnapable
    public override bool FindSnapPoint(ref Vector3 currentPos, float snapDistance)
    {
        if (((this.start - currentPos).sqrMagnitude < (this.end - currentPos).sqrMagnitude)
            && ((this.start - currentPos).magnitude < snapDistance))
        {
            currentPos = start;
            return true;
        }
        else if ((this.end - currentPos).magnitude < snapDistance)
        {
            currentPos = end;
            return true;
        }
        return false;
    }


    //Serialization
    [Serializable]
    public struct SerializableItem
    {
        public Vector3 start;
        public Vector3 end;
    }

    public SerializationData Serialize()
    {
        SerializationData tmp;

        this.serializableItem.start = this.start;
        this.serializableItem.end = this.end;
        tmp.type = SerializationController.ItemType.WallHandler;
        tmp.data = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        this.initFromSerialization = true;
        this.serializableItem = JsonUtility.FromJson<SerializableItem>(json);
    }
}
