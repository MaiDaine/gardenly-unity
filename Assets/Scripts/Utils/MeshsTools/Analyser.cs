using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analyser : MonoBehaviour
{
    private Mesh mesh = null;

    void Start()
    {
        GridController.instance.eventPostRender.AddListener(DrawLines);
        MeshFilter meshFilter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (meshFilter != null)
            mesh = meshFilter.mesh;
        else
            Destroy(this);
    }

    private void OnDestroy()
    {
        GridController.instance.eventPostRender.RemoveListener(DrawLines);
    }

    void DrawLines()
    {
        if (mesh != null)
            for (int i = 0; i < mesh.triangles.Length; i = i + 3)
            {
                Debug.DrawLine(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]], Color.yellow);
                Debug.DrawLine(mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]], Color.yellow);
                Debug.DrawLine(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 2]], Color.yellow);
            }
    }
}
