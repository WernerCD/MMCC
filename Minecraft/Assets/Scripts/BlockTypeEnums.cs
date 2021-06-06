public enum BlockTypeEnums : byte
{
    Air = 0,
    Bedrock = 1,
    Stone = 2,
    Grass = 3,
    Sand = 4,
    Dirt = 5,
    Furnace = 6,
}

public static class EnumExtensions
{
    public static byte ToByte(this BlockTypeEnums e) => (byte) e;
}