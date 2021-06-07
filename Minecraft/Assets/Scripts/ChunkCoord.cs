using UnityEngine;

/// <summary>
///   Location of chunk within chunky chunks
/// </summary>
public class ChunkCoord
{
    public int X;
    public int Z;

    public ChunkCoord()
    {
        X = 0;
        Z = 0;
    }

    public ChunkCoord(int x, int z)
    {
        X = x;
        Z = z;
    }

    public ChunkCoord(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);
        
        X = xCheck / VoxelData.ChunkXWidth;
        Z = zCheck / VoxelData.ChunkXWidth;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null) return false;
        if (other.X == X && other.Z == Z) return true;
        return false;
    }
}