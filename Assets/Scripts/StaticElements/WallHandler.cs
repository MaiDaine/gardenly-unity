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
    private WallTextHandler Text = null;
    private Vector3 start;
    private Vector3 end;
    private bool initFromSerialization = false;

    void Awake()
    {
        uIController = Camera.main.GetComponent<UIController>();
    }
    void Start()
    {
        if (initFromSerialization)
        {
            StartPreview(serializableItem.start);
            Preview(serializableItem.end);
            EndPreview();
        }
        else
        {
            gameObject.layer = 0;
            this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
        }
        SerializationController.instance.AddToList(this);
    }

    void OnDestroy()
    {
        if (Text != null)
            GameObject.Destroy(Text);
        SerializationController.instance.RemoveFromList(this);
    }

    public override void StartPreview(Vector3 position)
    {
        if (Text == null)
            Text = Instantiate(TextRef, this.transform.position, Quaternion.identity) as WallTextHandler;
        else
            Text.transform.position = position;
        Text.gameObject.SetActive(true);
        if (uIController.GetMenuScript() != null && uIController.GetMenuScript().isMoving)
            return;
        start = position;
    }

    public override void Preview(Vector3 position)
    {
        if (uIController.GetMenuScript() != null && uIController.GetMenuScript().isMoving)
        {
            start += (this.transform.position - position);
            end += (this.transform.position - position);
            this.transform.position = position;
            Text.transform.position = position;
            return;
        }
        if (start != position && end != position)
        {
            end = position;
            Vector3 tmp = (start + end) / 2f;
            float lenght = (start - end).magnitude;

            this.transform.position = tmp;
            this.transform.rotation = (Quaternion.LookRotation(end - start, Vector3.up) * Quaternion.Euler(0, 90, 0));
            this.transform.localScale = new Vector3(lenght, transform.localScale.y, transform.localScale.z);

            Text.transform.position = tmp;
            Text.SetText(string.Format("{0:F1}m", lenght));
        }
    }

    public override void EndPreview()
    {
        this.gameObject.layer = 10;
        Text.gameObject.SetActive(false);
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
        if (state == ConstructionController.ConstructionState.Off)
            uIController.SpawnDynMenu(this, uIController.wallMenu);

        Text.gameObject.SetActive(true);
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
        if (Text.gameObject != null)
            Text.gameObject.SetActive(false);
        // Inutile si le deselect supprime le menu
        if (uIController.GetMenuScript() != null && uIController.GetMenuScript().rotateState)
        {
            uIController.GetMenuScript().rotateState = false;
            uIController.GetMenuScript().GetComponentInChildren<LabelScript>().ResetColor();
        }
        // TODO si le menu bloque le ray cast appel destroymenu
        //uIController.GetMenuScript().DestroyMenu();
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


    //Serialization
    [Serializable]
    public struct SerializableItem
    {
        public Vector3 start;
        public Vector3 end;
    }

    public SerializationData Serialize()
    {
        serializableItem.start = start;
        serializableItem.end = end;
        SerializationData tmp;
        tmp.type = SerializationController.ItemType.WallHandler;
        tmp.data = JsonUtility.ToJson(serializableItem);
        return (tmp);
    }

    public void DeSerialize(string json)
    {
        initFromSerialization = true;
        serializableItem = JsonUtility.FromJson<SerializableItem>(json);
    }
}
