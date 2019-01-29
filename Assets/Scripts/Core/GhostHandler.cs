using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GhostHandler : MonoBehaviour, ISelectable, ISnapable
{
    protected List<ISelectable> neighbors = new List<ISelectable>();

    void Start()
    {
        this.gameObject.layer = 0;
    }

    void OnDestroy()
    {
        for (int cnt = 0; cnt < neighbors.Count; cnt++)
            neighbors[cnt].RemoveFromNeighbor(this);
    }

    void Update()
    {
    }

    public virtual void StartPreview(Vector3 position)
    {
    }

    public virtual void Preview(Vector3 position)
    {
        this.transform.position = position;
    }

    public virtual void EndPreview()
    {
        this.gameObject.layer = 10;
    }

    public void Rotate(float axisInput)
    {
        Vector3 rotateValue = new Vector3(0, (axisInput * 10f), 0);
        this.transform.eulerAngles += rotateValue;
    }


    //ISelectable && ISnapable
    public GameObject GetGameObject() { return (this.gameObject); }

    //ISelectable
    public virtual void Select(ConstructionController.ConstructionState state)
    {
    }

    public virtual List<ISelectable> SelectWithNeighbor()
    {
        Select(ConstructionController.ConstructionState.Off);
        List<ISelectable> tmp = new List<ISelectable>(neighbors);
        foreach (ISelectable item in tmp)
            item.Select(ConstructionController.ConstructionState.Off);
        return tmp;
    }

    public virtual void DeSelect()
    {
    }

    public void AddNeighbor(ISelectable item)
    {
        if (!neighbors.Contains(item))
            neighbors.Add(item);
    }

    public void RemoveFromNeighbor(ISelectable item)
    {
        neighbors.Remove(item);
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

    public virtual bool isLinkable()
    {
        return true;
    }
}   