using System;

[Serializable]
public class Lode
{
    public string NodeName;
    public byte BlockId;
    public int MinHeight;
    public int MaxHeight;
    public float Scale;
    public float Threshold;
    public float NoiseOffset;
}