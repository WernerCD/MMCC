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
    private readonly byte[,,] _voxelMap = new byte[VoxelData.ChunkXWidth, VoxelData.ChunkYHeight, VoxelData.ChunkZDepth];

    private World _world;
    private int _vertexIndex = 0;

    void Start()
    {
        _world = GameObject.Find("World").GetComponent<World>();
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
            if (y < 1) 
                _voxelMap[x, y, z] = 1; // Bedrock, Bottom
            else if (y == VoxelData.ChunkYHeight-1) 
                _voxelMap[x, y, z] = 3; // Grass, Top
            else 
                _voxelMap[x, y, z] = 2; // stone, middle
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

            var blockId = _voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
            AddTexture(_world.BlockTypes[blockId].GetTextureID(p));

            _triangles.Add(_vertexIndex + 0);
            _triangles.Add(_vertexIndex + 1);
            _triangles.Add(_vertexIndex + 2);

            _triangles.Add(_vertexIndex + 2);
            _triangles.Add(_vertexIndex + 1);
            _triangles.Add(_vertexIndex + 3);

            _vertexIndex += 4;
        }
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

        return _world.BlockTypes[_voxelMap[x, y, z]].IsSolid;
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

    private void AddTexture(int textureId)
    {
        float y = textureId / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureId - (y * VoxelData.TextureAtlasSizeInBlocks);
        Debug.Log($"ID: {textureId}, N: {VoxelData.TextureAtlasSizeInBlocks}, X: {x}, Y: {y}");

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        Debug.Log($"ID: {textureId}, N: {VoxelData.TextureAtlasSizeInBlocks}, X: {x}, Y: {y}");
        _uvs.Add(new Vector2(x, y));
        _uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y+ VoxelData.NormalizedBlockTextureSize));
    }
}
