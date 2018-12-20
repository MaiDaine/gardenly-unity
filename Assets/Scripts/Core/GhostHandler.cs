using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHandler : MonoBehaviour, ISelectable, ISnapable
{
    private List<ISelectable> neighbors = new List<ISelectable>();

    void Start()
    {
        gameObject.layer = 0;
    }

    void OnDestroy()
    {
        foreach (ISelectable elem in neighbors)
            elem.RemoveFromNeighbor(this);
    }

    void Update()
    {
    }

    public void StartPreview(Vector3 position)
    {
    }

    public void EndPreview()
    {
        gameObject.layer = 9;
    }

    public void Preview(Vector3 position)
    {
        transform.position = position;
    }


    //ISelectable && ISnapable
    public GameObject GetGameObject() { return (this.gameObject); }

    //ISelectable
    public void Select()
    {
    }

    public List<ISelectable> SelectWithNeighbor()
    {
        Select();
        List<ISelectable> tmp = new List<ISelectable>(neighbors);
        foreach (ISelectable item in tmp)
            item.Select();
        return tmp;
    }

    public void DeSelect()
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
    public bool FindSnapPoint(ref Vector3 currentPos, float snapDistance)
    {
        if ((currentPos - transform.position).magnitude < snapDistance)
        {
            currentPos = transform.position;
            return true;
        }
        return false;
    }

    public bool isLinkable()
    {
        return true;
    }
}