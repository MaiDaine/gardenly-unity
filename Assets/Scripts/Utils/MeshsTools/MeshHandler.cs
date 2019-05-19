using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHandler : MonoBehaviour
{
    private Mesh mesh;

    public Mesh Init(Vector2[] vertices2D, int qualitySettings = 0)
    {
        mesh = new Mesh();
        List<Vector2> points = new List<Vector2>(vertices2D);
        List<int> indices = null;
        List<Vector3> vertices = null;

        Triangulate.triangulate(points, null, out indices, out vertices);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.Optimize();

        GenerateUV();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    void GenerateUV()
    {
        if (mesh.vertices.Length == 0)
            return;
        Vector2 min = new Vector2(mesh.vertices[0].x, mesh.vertices[0].y);
        Vector2 max = new Vector2(0f, 0f);
        Vector2 dist = new Vector2(0f, 0f);

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (mesh.vertices[i].x < min.x)
                min.x = mesh.vertices[i].x;
            else if (mesh.vertices[i].x > max.x)
                max.x = mesh.vertices[i].x;

            if (mesh.vertices[i].z < min.y)
                min.y = mesh.vertices[i].z;
            else if (mesh.vertices[i].z > max.y)
                max.y = mesh.vertices[i].z;
        }

        dist.x = max.x - min.x;
        dist.y = max.y - min.y;

        Vector2[] updatedUV = new Vector2[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            updatedUV[i].x = ((mesh.vertices[i].x - min.x) / dist.x);
            updatedUV[i].y = ((mesh.vertices[i].z - min.y) / dist.y);
        }
        mesh.uv = updatedUV;
    }

    void ApplyQuality()
    {
        ImproveMeshQuality improve = new ImproveMeshQuality();
        int vOffset = mesh.vertices.Length;
        int[] updatedTriangles = new int[mesh.triangles.Length * 3];
        int triangleAdd = mesh.triangles.Length / 3;

        mesh.vertices = improve.ExpandVertices(mesh.vertices, triangleAdd);

        Vector3[] tmpVertices = new Vector3[mesh.vertices.Length];
        mesh.vertices.CopyTo(tmpVertices, 0);

        improve.ImproveQuality(ref tmpVertices, vOffset, ref updatedTriangles, mesh.triangles, 0.1f);
        mesh.vertices = tmpVertices;
        mesh.triangles = updatedTriangles;
    }
}
