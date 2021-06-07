using UnityEngine;

public class Noise 
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="offest"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static float Get2DPerlin(Vector2 pos, float offset, float scale)
    {
        return Mathf.PerlinNoise((pos.x + 0.1f) / VoxelData.ChunkXWidth * scale + offset, (pos.y + 0.1f) / VoxelData.ChunkXWidth * scale + offset);
   }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="offset"></param>
    /// <param name="scale"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    /// <seealso xref="https://www.youtube.com/watch?v=Aga0TBJkchM"/>
    public static bool Get3DPerlin(Vector3 pos, float offset, float scale, float threshold)
    {
        float x = (pos.x + offset + 0.1f) * scale;
        float y = (pos.y + offset + 0.1f) * scale;
        float z = (pos.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x: x, y: y);
        float BC = Mathf.PerlinNoise(x: y, y: z);
        float AC = Mathf.PerlinNoise(x: x, y: z);
        float BA = Mathf.PerlinNoise(x: y, y: x);
        float CB = Mathf.PerlinNoise(x: z, y: y);
        float CA = Mathf.PerlinNoise(x: z, y: x);

        return (AB + BC + AC + BA + CB + CA) / 6f > threshold;
    }
}
