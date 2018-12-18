using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Material))]
public class WallHandler : MonoBehaviour, ISelectable, ISnapable
{
    public WallTextHandler TextRef;

    private WallTextHandler Text = null;
    private Vector3 start;
    private Vector3 end;
    private List<ISelectable> neighbors = new List<ISelectable>();

    void Start()
    {
        gameObject.layer = 0;
        this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
    }

    void OnDestroy()
    {
        if (Text != null)
            GameObject.Destroy(Text);
        foreach (ISelectable elem in neighbors)
            elem.RemoveFromNeighbor(this);
    }

    void Update()
    {
        
    }

    public void StartPreview(Vector3 position)
    {
        Text = Instantiate(TextRef, this.transform.position, Quaternion.identity) as WallTextHandler;
        Text.gameObject.SetActive(true);
        start = position;
    }


    public void EndPreview()
    {
        gameObject.layer = 9;
        Text.gameObject.SetActive(false);
    }

    public void Preview(Vector3 position)
    {
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


    //ISelectable && ISnapable
    public GameObject GetGameObject() { return (this.gameObject); }
    
    //ISelectable
    public void Select()
    {
        Text.gameObject.SetActive(true);
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
        Text.gameObject.SetActive(false);
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

    public bool isLinkable()
    {
        return true;
    }
}
