/// <summary>
///   Location of chunk within chunky chunks
/// </summary>
public class ChunkCoord
{
    public int X;
    public int Z;

    public ChunkCoord(int x, int z)
    {
        X = x;
        Z = z;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null) return false;
        if (other.X == X && other.Z == Z) return true;
        return false;
    }
}