using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Transform Player;
    public Vector3 SpawnPosition;
    public Material Material;
    public BlockType[] BlockTypes;
    public int Seed;
    public BiomeAttribute Biome;


    private Chunk[,] _chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    private List<ChunkCoord> _activeChunks = new List<ChunkCoord>();
    private ChunkCoord _playerChunkCoord;
    private ChunkCoord _playerLastChunkCoord;

    private void Start()
    {
        Random.InitState(Seed);
        SpawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkXWidth) / 2f, VoxelData.ChunkYHeight, (VoxelData.WorldSizeInChunks * VoxelData.ChunkXWidth) / 2f);
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

    public bool CheckForVoxel(float x, float y, float z)
    {
        int xCheck = Mathf.FloorToInt(x);
        int yCheck = Mathf.FloorToInt(y);
        int zCheck = Mathf.FloorToInt(z);

        int xChunk = xCheck / VoxelData.ChunkXWidth;
        int yChunk = yCheck / VoxelData.ChunkYHeight;
        int zChunk = zCheck / VoxelData.ChunkXWidth;

        xCheck -= (xChunk * VoxelData.ChunkXWidth);
        zCheck -= (zChunk * VoxelData.ChunkXWidth);

        return BlockTypes[_chunks[xChunk, zChunk]._voxelMap[xCheck, yCheck, zCheck]].IsSolid;
    }

    public byte GetVoxel(Vector3 pos)
    {

        int xPos = Mathf.FloorToInt(pos.x);
        int yPos = Mathf.FloorToInt(pos.y);
        int zPos = Mathf.FloorToInt(pos.z);

        /* Immutable Pass */

        // if outside the world, air
        if (!IsVoxelInWorld(pos)) return BlockTypeEnums.Air.ToByte();

        // bottom chunk, bedrock
        if (yPos == 0) return BlockTypeEnums.Bedrock.ToByte();

        /* Basic Terrain Pass */

        int terrainHeight = Mathf.FloorToInt(VoxelData.ChunkYHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, Biome.TerrainScale));
        byte voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = BlockTypeEnums.Grass.ToByte();
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = BlockTypeEnums.Dirt.ToByte();
        else if (yPos > terrainHeight)
            voxelValue = BlockTypeEnums.Air.ToByte();
        else
            voxelValue = BlockTypeEnums.Stone.ToByte();

        /* Second Pass */
        if (voxelValue == 2)
        {
            foreach (var lode in Biome.Lodes)
            {
                if (yPos > lode.MinHeight && yPos < lode.MaxHeight)
                    if (Noise.Get3DPerlin(pos, lode.NoiseOffset, lode.Scale, lode.Threshold))
                        voxelValue = lode.BlockId;
            }
        }
        return voxelValue;
    }

    void CreateNewChunk(int x, int z)
    {
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