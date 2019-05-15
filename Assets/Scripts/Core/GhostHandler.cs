using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GhostHandler : MonoBehaviour, ISelectable, ISnapable
{
    public bool needFlowerBed = false;
    protected PlantData data = null;
    protected List<ISelectable> neighbors = new List<ISelectable>();
    //TODO UI : add UI controller here 

    void Start()
    {
        this.gameObject.layer = 0;
    }

    void OnDestroy()
    {
        for (int i = 0; i < this.neighbors.Count; i++)
            this.neighbors[i].RemoveFromNeighbor(this);
    }

    public virtual bool OnCancel()
    {
       // Camera.main.GetComponent<UIController>().Cancel();
        return true;
    }

    public virtual void Positioning(Vector3 position) { this.transform.position = position; }

    public virtual bool FromPositioningToBuilding(Vector3 position)
    {
        Positioning(position);
        return true;
    }

    public virtual bool Building(Vector3 position) { return true; }

    public virtual void EndConstruction(Vector3 position) { this.gameObject.layer = 10; }

    public virtual void Move(Vector3 position) { this.transform.position = position; }

    public virtual void Rotate(float input)
    {
        float rotx = input * 100f * Mathf.Deg2Rad;

        if (this.transform.localEulerAngles.x <= 270)
            this.transform.Rotate(Vector3.up, -rotx);
        else
            this.transform.Rotate(Vector3.forward, -rotx);
    }

    public void FixRotate(float axisInput)
    {
        Vector3 rotateValue = new Vector3(0, (axisInput * 10f), 0);
        this.transform.eulerAngles += rotateValue;
    }

    public PlantData GetData()
    {
        return this.data;
    }

    public void SetData(PlantData data)
    {
        if (data != null)
            this.data = data;
    }


    //ISelectable && ISnapable
    public GameObject GetGameObject() { return (this.gameObject); }

    //ISelectable
    public virtual void Select(ConstructionController.ConstructionState state) { }

    public virtual List<ISelectable> SelectWithNeighbor()
    {
        Select(ConstructionController.ConstructionState.Off);
        List<ISelectable> tmp = new List<ISelectable>(this.neighbors);
        foreach (ISelectable item in tmp)
            item.Select(ConstructionController.ConstructionState.Off);
        return tmp;
    }

    public virtual void DeSelect() { }

    public void AddNeighbor(ISelectable item)
    {
        if (!this.neighbors.Contains(item))
            this.neighbors.Add(item);
    }

    public void RemoveFromNeighbor(ISelectable item)
    {
        this.neighbors.Remove(item);
    }

    //ISnapable
    public virtual bool FindSnapPoint(ref Vector3 currentPos, float snapDistance)
    {
        if ((currentPos - this.transform.position).magnitude < snapDistance)
        {
            currentPos = this.transform.position;
            return true;
        }
        return false;
    }

    public virtual bool isLinkable() { return true; }

    protected virtual void OnEnable()
    {
        PlayerController.instance.SelectFromAction(this.GetComponent<ISelectable>());
    }

    protected virtual void OnDisable()
    {
        DeSelect();
    }
}