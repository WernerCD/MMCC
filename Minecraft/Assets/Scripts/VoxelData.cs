using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkXWidth = 16;
    public static readonly int ChunkYHeight = 128;
    public static readonly int WorldSizeInChunks = 10;
    public static readonly int ViewDistanceInChunks = 3;

    //public static readonly int ChunkZDepth = 5;
    
    public static readonly int TextureAtlasSizeInBlocks = 16;
    
    public static float NormalizedBlockTextureSize => 1f / (float) TextureAtlasSizeInBlocks;
    public static int WorldSizeInVoxels => WorldSizeInChunks * ChunkXWidth;

    public static readonly Vector3[] VoxelVerts = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),
    };

    public static readonly Vector3[] FaceChecks = new Vector3[6]
    {
        new Vector3(0.0f, 0.0f, -1.0f), // Back
        new Vector3(0.0f, 0.0f, 1.0f),  // Front 
        new Vector3(0.0f, 1.0f, 0.0f),  // Top
        new Vector3(0.0f, -1.0f, 0.0f), // Bottom
        new Vector3(-1.0f, 0.0f, 0.0f), // Left 
        new Vector3(1.0f, 0.0f, 0.0f),  // Right
    };

    public static readonly int[,] VoxelTris = new int[6, 4]
    {
        {0, 3, 1, 2}, // Back
        {5, 6, 4, 7}, // Front 
        {3, 7, 2, 6}, // Top
        {1, 5, 0, 4}, // Bottom
        {4, 7, 0, 3}, // Left
        {1, 2, 5, 6}, // Right
    };

    public static readonly Vector2[] VoxelUvs = new Vector2[4]
    {
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 1.0f),
    };
}
