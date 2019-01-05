using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBedHandler : MonoBehaviour
{
    public Material material;
    private MeshHandler meshHandler;
    private List<Vector2> vertices2D = new List<Vector2>();

    void Start()
    {
        meshHandler = gameObject.AddComponent<MeshHandler>();
    }

    public void AddPoint(Vector2 point)
    {
        vertices2D.Add(point);
    }
    
    public void Init()
    {
        MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = meshHandler.Init(vertices2D.ToArray(), 0);
        renderer.material = material;
        //this.transform.eulerAngles += new Vector3(0, 0, 180);
    }

    void Update()
    {
    }   
}
