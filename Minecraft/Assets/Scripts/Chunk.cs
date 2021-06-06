using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord Coord;
    
    private readonly World _world;
    private readonly GameObject _chunkObject;
    private readonly MeshRenderer _meshRenderer;
    private readonly MeshFilter _meshFilter;

    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();
    private readonly List<Vector2> _uvs = new List<Vector2>();
    private readonly byte[,,] _voxelMap = new byte[VoxelData.ChunkXWidth, VoxelData.ChunkYHeight, VoxelData.ChunkXWidth];

    private int _vertexIndex = 0;
    
    public bool IsActive
    {
        get { return _chunkObject.activeSelf; }
        set { _chunkObject.SetActive(value); }
    }

    public Vector3 position => _chunkObject.transform.position;

    public Chunk(World world, ChunkCoord coord)
    {
        Coord = coord;

        _world = world;
        _chunkObject = new GameObject();
        _meshFilter = _chunkObject.AddComponent<MeshFilter>();
        _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();

        _meshRenderer.material = _world.Material;
        _chunkObject.transform.SetParent(world.transform);
        _chunkObject.transform.position = new Vector3(coord.X * VoxelData.ChunkXWidth,0f, coord.Z * VoxelData.ChunkXWidth);
        _chunkObject.name = $"Chunk: {coord.X},{coord.Z}";

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }
    
    void PopulateVoxelMap()
    {
        for (int x = 0; x < VoxelData.ChunkXWidth; x++)
        for (int y = 0; y < VoxelData.ChunkYHeight; y++)
        for (int z = 0; z < VoxelData.ChunkXWidth; z++)
        {
            _voxelMap[x, y, z] = _world.GetVoxel(new Vector3(x, y, z) + position);
        }
    }
    
    void CreateMeshData()
    {
        for (int x = 0; x < VoxelData.ChunkXWidth; x++)
        for (int y = 0; y < VoxelData.ChunkYHeight; y++)
        for (int z = 0; z < VoxelData.ChunkXWidth; z++)
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

        if (!IsVoxelInChunk(x, y, z))
            return _world.BlockTypes[_world.GetVoxel(pos + position)].IsSolid;

        return _world.BlockTypes[_voxelMap[x, y, z]].IsSolid;
    }

    bool IsVoxelInChunk(int x, int y, int z) => 
        x >= 0 && x <= VoxelData.ChunkXWidth -1 &&
        y >= 0 && y <= VoxelData.ChunkYHeight -1 &&
        z >= 0 && z <= VoxelData.ChunkXWidth -1 ;

    private void CreateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = _uvs.ToArray();

        mesh.RecalculateNormals();

        _meshFilter.mesh = mesh;
    }

    private void AddTexture(int textureId)
    {
        float y = textureId / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureId - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;
        
        _uvs.Add(new Vector2(x, y));
        _uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y+ VoxelData.NormalizedBlockTextureSize));
    }
}
