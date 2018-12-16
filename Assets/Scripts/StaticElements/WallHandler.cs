using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Material))]
public class WallHandler : MonoBehaviour, ISelectable, ISnapable
{
    public WallTextHandler Text;
    private Vector3 start;
    private Vector3 end;

    void Start()
    {
        gameObject.layer = 0;
        this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
    }

    void Update()
    {
        
    }

    public void StartPreview(Vector3 position)
    {
        Text = Instantiate(Text, this.transform.position, Quaternion.identity) as WallTextHandler;
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


    //ISelectable
    public void Select()
    {
        Text.gameObject.SetActive(true);
    }

    public void DeSelect()
    {
        Text.gameObject.SetActive(false);
    }

    //ISnapable
    public Vector3 FindSnapPoint(Vector3 currentPos, float snapDistance)
    {
        if ((start - currentPos).sqrMagnitude < (end - currentPos).sqrMagnitude)
        {
            if ((start - currentPos).magnitude < snapDistance)
                return start;
            return currentPos;
        }
        if ((end - currentPos).magnitude < snapDistance)
            return end;
        return currentPos;
    }
}
