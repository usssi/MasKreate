using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaskElementData
{
    public string spriteDataName; // Name of the SpriteData ScriptableObject
    public Vector3 localPosition;
    public float widthOffset;
    public float heightOffset;
    public float scale;
    public float zRotation;
    public Color color;
}

[Serializable]
public class MaskDesignData
{
    public List<MaskElementData> elements = new List<MaskElementData>();
}
