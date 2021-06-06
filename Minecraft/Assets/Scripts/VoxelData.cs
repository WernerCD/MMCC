using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData
{
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

    public static readonly int[,] VoxelTris = new int[6, 6]
    {
        {0, 3, 1, 1, 3, 2}, // Back
        {5, 6, 4, 4, 6, 7}, // Front 
        {3, 7, 2, 2, 7, 6}, // Top
        {1, 5, 0, 0, 5, 4}, // Bottom
        {4, 7, 0, 0, 7, 3}, // Left
        {1, 2, 5, 5, 2, 6}, // Right
    };

    public static readonly Vector2[] VoxelUvs = new Vector2[6]
    {
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
    };
}
