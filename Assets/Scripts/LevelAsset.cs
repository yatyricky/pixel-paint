using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LevelAsset : ScriptableObject
{
    public int Width;
    public int Height;

    public int PaletteSize;
    public int DataSize;
    [HideInInspector] public Color[] Palette;
    [HideInInspector] public Color[] Data;
}
