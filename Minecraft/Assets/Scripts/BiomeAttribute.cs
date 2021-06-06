using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Minecraft Tutorial/Biome Attributes")]
public class BiomeAttribute : ScriptableObject
{
    public string biomeName;
    public int SolidGroundHeight;
    public int TerrainHeight;
    public float TerrainScale;

    public Lode[] Lodes;
}