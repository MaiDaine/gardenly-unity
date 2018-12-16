using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Material))]
public class WallHandler : MonoBehaviour
{
    public WallTextHandler Text;
    private Vector3 start;
    private Vector3 end;

    void Start()
    {
        this.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
    }

    void Update()
    {
        
    }

    public void StartPreview(Vector3 position)
    {
        Text = Instantiate(Text, this.transform.position, Quaternion.identity) as WallTextHandler;
        start = position;
    }


    public void EndPreview()
    {
        gameObject.layer = 9;
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
}
