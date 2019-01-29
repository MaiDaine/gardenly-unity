using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHandler : MonoBehaviour
{
    private Mesh mesh;

    public Mesh Init(Vector2[] vertices2D, int qualitySettings = 0)
    {

        mattatz.Triangulation2DSystem.Polygon2D polygon = mattatz.Triangulation2DSystem.Polygon2D.Contour(vertices2D);
        mattatz.Triangulation2DSystem.Triangulation2D triangulation = new mattatz.Triangulation2DSystem.Triangulation2D(polygon, 22.5f);
        mesh = triangulation.Build();

        for (int i = 0; i < qualitySettings; i++)
            ApplyQuality();
        GenerateUV();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        return mesh;
    }

    public Mesh AddPoint(Vector3 point, int[] triangles)
    {
        Vector3[] tmpV = new Vector3[mesh.vertexCount + 1];
        Array.Copy(mesh.vertices, tmpV, mesh.vertexCount);
        tmpV[mesh.vertexCount] = point;
        int[] tmpT = new int[mesh.triangles.Length + 3];
        Array.Copy(mesh.triangles, tmpT, mesh.triangles.Length);
        tmpT[mesh.triangles.Length] = triangles[0];
        tmpT[mesh.triangles.Length + 1] = triangles[1];
        tmpT[mesh.triangles.Length + 2] = triangles[2];
        mesh.vertices = tmpV;
        mesh.triangles = tmpT;
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
