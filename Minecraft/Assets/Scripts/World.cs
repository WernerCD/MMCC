using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform Player;
    public Vector3 SpawnPosition;
    public Material Material;
    public BlockType[] BlockTypes;

    private Chunk[,] _chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();
    private ChunkCoord _playerChunkCoord;
    private ChunkCoord _playerLastChunkCoord;

    private void Start()
    {
        SpawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkXWidth) / 2f, VoxelData.ChunkYHeight+2, (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks / 2f);
        GenerateWorld();
        _playerLastChunkCoord = GetChunkCoordFromVector3(Player.position);
    }

    private void Update()
    {
        _playerChunkCoord = GetChunkCoordFromVector3(Player.position);
        if (!_playerChunkCoord.Equals(_playerLastChunkCoord))
        {
            CheckViewDistance();
            _playerLastChunkCoord = _playerChunkCoord;
        }
    }

    void GenerateWorld()
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
        {
            CreateNewChunk(x, z);
        }
        Player.position = SpawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        var x = Mathf.FloorToInt((pos.x / VoxelData.ChunkXWidth));
        var z = Mathf.FloorToInt((pos.z / VoxelData.ChunkXWidth));
        return new ChunkCoord(x, z);
    }

    void CheckViewDistance()
    {
        var coord = GetChunkCoordFromVector3(Player.position);
        var previouslyActiveChunks = new List<ChunkCoord>(_activeChunks);

        for (int x = coord.X - VoxelData.ViewDistanceInChunks; x < coord.X + VoxelData.ViewDistanceInChunks; x++)
        for (int z = coord.Z - VoxelData.ViewDistanceInChunks; z < coord.Z + VoxelData.ViewDistanceInChunks; z++)
        {
            var chunkCoord = new ChunkCoord(x, z);
            if (IsChunkInWorld(chunkCoord))
                if (_chunks[x, z] == null)
                {
                    CreateNewChunk(x, z);
                }
                else if (!_chunks[x, z].IsActive)
                {
                    _chunks[x, z].IsActive = true;
                    _activeChunks.Add(chunkCoord);
                }

            for (var i = 0; i < previouslyActiveChunks.Count;)
            {
                if (previouslyActiveChunks[i].Equals(chunkCoord))
                    previouslyActiveChunks.RemoveAt(i);
                else
                    i++;
            }
        }

        foreach (var c in previouslyActiveChunks)
        {
            _chunks[c.X, c.Z].IsActive = false;
        }
    }

    public byte GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
            return 0;

        if (pos.y < 1)
            return 1; // Bedrock, Bottom
        else if (Mathf.FloorToInt(pos.y) == VoxelData.ChunkYHeight - 1)
            return 3; // Grass, Top
        else
            return 2; // stone, middle
    }

    void CreateNewChunk(int x, int z)
    {
        Debug.Log($"Create: {x}, {z}");
        _chunks[x, z] = new Chunk(this, new ChunkCoord(x, z));
        _activeChunks.Add(new ChunkCoord(x, z));
    }

    bool IsChunkInWorld(ChunkCoord coord) =>
        coord.X >= 0 && coord.X < VoxelData.WorldSizeInChunks &&
        coord.Z >= 0 && coord.Z < VoxelData.WorldSizeInChunks;

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels &&
            pos.y >= 0 && pos.y < VoxelData.ChunkYHeight &&
            pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels) 
            return true;
        else
            return false;
    }
}