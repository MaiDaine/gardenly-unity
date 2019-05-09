using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapePoint : MonoBehaviour, IInteractible, ISnapable
{
    public float overrideSnapDistance = 0.5f;

    public void ChangeColor(Color color)
    {
        this.GetComponent<MeshRenderer>().material.color = color;
    }

    private void Awake() { gameObject.layer = 0; }
    
    public void EndConstruction() { gameObject.layer = 10; }

    // IInteractible
    public GameObject GetGameObject() { return this.gameObject; }
    public void Activate() { this.gameObject.SetActive(true); }
    public void DeActivate() { this.gameObject.SetActive(false); }
    public void DragClick(Vector3 endPosition) { this.transform.position = endPosition; }
    public void EndDrag() { }

    //ISnapable
    //public GameObject GetGameObject() { return this.gameObject; }
    public bool FindSnapPoint(ref Vector3 currentPos, float snapDistance)
    {
        if (Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(currentPos.x, currentPos.z)) < overrideSnapDistance)
        {
            currentPos = this.transform.position;
            return true;
        }
        return false;
    }
    public bool isLinkable() { return false; }
}
