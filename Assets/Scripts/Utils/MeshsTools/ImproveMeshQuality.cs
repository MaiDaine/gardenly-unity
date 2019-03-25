using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImproveMeshQuality
{
    public Vector3[] ExpandVertices(Vector3[] vertices, int triangleAdd)
    {
        Vector3[] result = new Vector3[vertices.Length + triangleAdd];
        vertices.CopyTo(result, 0);
        return result;
    }

    public Vector2[] ExpandUV(Vector2[] uv, int triangleAdd)
    {
        Vector2[] result = new Vector2[uv.Length + triangleAdd];
        uv.CopyTo(result, 0);
        return result;
    }

    public void ImproveQuality(ref Vector3[] vertices, int vOffset, ref int[] triangles, int[] oldTriangles, float yPos = 0.1f)
    {
        int tOffset = 0;

        for (int i = 0; i < oldTriangles.Length; i = i + 3)
        {
            Vector3 center = new Vector3(0, yPos, 0);
            center.x = (vertices[oldTriangles[i]].x + vertices[oldTriangles[i + 1]].x + vertices[oldTriangles[i + 2]].x) / 3.0f;
            center.z = (vertices[oldTriangles[i]].z + vertices[oldTriangles[i + 1]].z + vertices[oldTriangles[i + 2]].z) / 3.0f;
            vertices[vOffset] = center;

            triangles[tOffset] = oldTriangles[i];
            triangles[tOffset + 1] = oldTriangles[i + 1];
            triangles[tOffset + 2] = vOffset;
            triangles[tOffset + 3] = oldTriangles[i + 2];
            triangles[tOffset + 4] = oldTriangles[i];
            triangles[tOffset + 5] = vOffset;
            triangles[tOffset + 6] = oldTriangles[i + 1];
            triangles[tOffset + 7] = oldTriangles[i + 2];
            triangles[tOffset + 8] = vOffset;
            vOffset++;
            tOffset += 9;
        }
    }
}
