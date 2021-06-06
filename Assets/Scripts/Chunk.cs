using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;

    void Start()
    {
        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int p = 0; p < 6; p++)
        for (int i = 0; i < 6; i++)
        {
            int triangleIndex = VoxelData.VoxelTris[p, i];
            vertices.Add(VoxelData.VoxelVerts[triangleIndex]);
            triangles.Add(vertexIndex);
            uvs.Add(VoxelData.VoxelUvs[i]);
            vertexIndex++;
        }

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        MeshFilter.mesh = mesh;
    }

}
