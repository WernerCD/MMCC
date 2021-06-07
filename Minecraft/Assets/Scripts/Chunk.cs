using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord Coord;
    public readonly byte[,,] VoxelMap = new byte[VoxelData.ChunkXWidth, VoxelData.ChunkYHeight, VoxelData.ChunkXWidth];
    public bool IsVoxelMapPopulated = false;

    private readonly World _world;
    private GameObject _chunkObject;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    private readonly List<Vector3> _vertices = new List<Vector3>();
    private readonly List<int> _triangles = new List<int>();
    private readonly List<Vector2> _uvs = new List<Vector2>();

    private int _vertexIndex = 0;
    

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_chunkObject != null)
                _chunkObject.SetActive(value);
        }
    }

    public Vector3 position => _chunkObject.transform.position;

    public Chunk(World world, ChunkCoord coord, bool generateOnLoad)
    {
        Coord = coord;
        _world = world;
        IsActive = true;
        if (generateOnLoad)
            Init();
    }

    public void Init()
    {

        _chunkObject = new GameObject();
        _meshFilter = _chunkObject.AddComponent<MeshFilter>();
        _meshRenderer = _chunkObject.AddComponent<MeshRenderer>();
        //_meshCollider = _chunkObject.AddComponent<MeshCollider>();

        _meshRenderer.material = _world.Material;
        _chunkObject.transform.SetParent(_world.transform);
        _chunkObject.transform.position = new Vector3(Coord.X * VoxelData.ChunkXWidth, 0f, Coord.Z * VoxelData.ChunkXWidth);
        _chunkObject.name = $"Chunk: {Coord.X},{Coord.Z}";

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
            VoxelMap[x, y, z] = _world.GetVoxel(new Vector3(x, y, z) + position);
        }
        IsVoxelMapPopulated = true;
    }

    void CreateMeshData()
    {
        for (int x = 0; x < VoxelData.ChunkXWidth; x++)
            for (int y = 0; y < VoxelData.ChunkYHeight; y++)
                for (int z = 0; z < VoxelData.ChunkXWidth; z++)
                {
                    if (_world.BlockTypes[VoxelMap[x, y, z]].IsSolid)
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

            var blockId = VoxelMap[(int)pos.x, (int)pos.y, (int)pos.z];
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
            return _world.CheckForVoxel(pos + position);

        return _world.BlockTypes[VoxelMap[x, y, z]].IsSolid;
    }

    public byte GetVoxelFromGlobalVector3(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(_chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(_chunkObject.transform.position.z);

        return VoxelMap[xCheck, yCheck, zCheck];
    }

    bool IsVoxelInChunk(int x, int y, int z) =>
        x >= 0 && x <= VoxelData.ChunkXWidth - 1 &&
        y >= 0 && y <= VoxelData.ChunkYHeight - 1 &&
        z >= 0 && z <= VoxelData.ChunkXWidth - 1;

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
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }
}
