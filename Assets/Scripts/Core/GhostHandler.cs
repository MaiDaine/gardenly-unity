using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public abstract class GhostHandler : MonoBehaviour, ISelectable, ISnapable
{
    public bool needFlowerBed = false;
    protected PlantData data = null;
    protected List<ISelectable> neighbors = new List<ISelectable>();

    private void Start()
    {
        this.gameObject.layer = 0;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < this.neighbors.Count; i++)
            this.neighbors[i].RemoveFromNeighbor(this);
    }


    public virtual bool OnCancel()
    {
        return true;
    }

    public virtual void Positioning(Vector3 position) { this.transform.position = position; }

    public virtual bool FromPositioningToBuilding(Vector3 position)
    {
        Positioning(position);
        return true;
    }

    public virtual bool Building(Vector3 position) { return true; }

    public virtual void EndConstruction(Vector3 position)
    {
        UIController uIController = Camera.main.GetComponent<UIController>();
        this.gameObject.layer = 10;
        uIController.ResetButton();
        if (uIController.GetCurrentHideView() != null && uIController.GetCurrentHideView().Count > 0)
        {
            foreach (UIView view in uIController.GetCurrentHideView())
                view.Show();
            uIController.GetCurrentHideView().Clear();
        }
    }

    //Actions
    public virtual void StartAction() { this.gameObject.layer = 0; }

    public virtual void EndAction() { this.gameObject.layer = 10; }

    public virtual void Move(Vector3 position) { this.transform.position = position; }

    public virtual void Rotate(float input)
    {
        float rotx = input * 100f * Mathf.Deg2Rad;

        if (this.transform.localEulerAngles.x <= 270)
            this.transform.Rotate(Vector3.up, -rotx);
        else
            this.transform.Rotate(Vector3.forward, -rotx);
    }

    //TODO #74
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

    //Call after Actions
    protected virtual void OnEnable()
    {
        if (!SpawnController.instance.loadingData)
            PlayerController.instance.SelectFromAction(this.GetComponent<ISelectable>());
    }

    protected virtual void OnDisable()
    {
        DeSelect();
    }
}