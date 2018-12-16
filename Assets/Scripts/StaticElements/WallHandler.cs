using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Material))]
public class WallHandler : MonoBehaviour
{
    MeshFilter mesh;

    void Start()
    {
        //tmp test
        mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(Vector3 position)
    {
        
    }
}
