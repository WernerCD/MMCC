using System;
using UnityEngine;

[Serializable]
public class BlockType
{
    public string BlockName;
    public bool IsSolid;
    
    // back, front, top, bottom, left, right
    [Header("Texture Values")]
    public int BackFaceTexture;
    public int FrontFaceTexture;
    public int TopFaceTexture;
    public int BottomFaceTexture;
    public int LeftFaceTexture;
    public int RightFaceTexture;


    public int GetTextureID(int faceIndex)
    {
        switch (faceIndex)
        {
            case 0: return BackFaceTexture;
            case 1: return FrontFaceTexture;
            case 2: return TopFaceTexture;
            case 3: return BottomFaceTexture;
            case 4: return LeftFaceTexture;
            case 5: return RightFaceTexture;
            default:
                Debug.Log("Illegal face");
                return -1;
        }
    }
}