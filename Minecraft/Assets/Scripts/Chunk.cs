using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;

    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly bool[,,] _voxelMap = new bool[VoxelData.ChunkXWidth, VoxelData.ChunkYHeight, VoxelData.ChunkZDepth];

    private int _vertexIndex = 0;

    void Start()
    {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }
    
    void PopulateVoxelMap()
    {
        for (int x = 0; x < VoxelData.ChunkXWidth; x++)
        for (int y = 0; y < VoxelData.ChunkYHeight; y++)
        for (int z = 0; z < VoxelData.ChunkZDepth; z++)
        {
            _voxelMap[x, y, z] = true;
        }
    }
    
    void CreateMeshData()
    {
        for (int x = 0; x < VoxelData.ChunkXWidth; x++)
        for (int y = 0; y < VoxelData.ChunkYHeight; y++)
        for (int z = 0; z < VoxelData.ChunkZDepth; z++)
        {
            AddVoxelDataToChunk(new Vector3(x, y, z));
        }

    }
    
    void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (CheckVoxel(pos + VoxelData.FaceChecks[p])) continue;
            
            _vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[p, 0]]);
            _vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[p, 1]]);
            _vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[p, 2]]);
            _vertices.Add(pos + VoxelData.VoxelVerts[VoxelData.VoxelTris[p, 3]]);
            
            _uvs.Add(VoxelData.VoxelUvs[0]);
            _uvs.Add(VoxelData.VoxelUvs[1]);
            _uvs.Add(VoxelData.VoxelUvs[2]);
            _uvs.Add(VoxelData.VoxelUvs[3]);

            _triangles.Add(_vertexIndex + 0);
            _triangles.Add(_vertexIndex + 1);
            _triangles.Add(_vertexIndex + 2);

            _triangles.Add(_vertexIndex + 2);
            _triangles.Add(_vertexIndex + 1);
            _triangles.Add(_vertexIndex + 3);

            _vertexIndex += 4;
        }
    }

    private void CreateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = _uvs.ToArray();

        mesh.RecalculateNormals();

        MeshFilter.mesh = mesh;
    }
    
    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || x > VoxelData.ChunkXWidth - 1 ||
            y < 0 || y > VoxelData.ChunkYHeight - 1 ||
            z < 0 || z > VoxelData.ChunkZDepth - 1
        )
            return false;

        return _voxelMap[x, y, z];
    }
}
