using System;
using UnityEngine;

public class LevelAsset
{
    public string Name;
    public int Width;
    public int Height;
    public Color[] Palette;
    public Color[] Data;

    public LevelAsset(LevelData data)
    {
        Name = data.Name;
        Width = data.Width;
        Height = data.Height;

        if (data.Palette != null)
        {
            Palette = new Color[data.Palette.Length];
            for (int i = 0; i < Palette.Length; i++)
            {
                Color color;
                string hex = "#" + data.Palette[i];
                if (ColorUtility.TryParseHtmlString(hex, out color))
                {
                    Palette[i] = color;
                }
                else
                {
                    throw new Exception("ColorUtility is wrong!");
                }
            }
        }

        Data = new Color[data.Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            Color color;
            string hex = "#" + data.Data[i];
            if (ColorUtility.TryParseHtmlString("#" + data.Data[i], out color))
            {
                Data[i] = color;
            }
            else
            {
                Debug.Log(System.Environment.StackTrace);
                throw new Exception("ColorUtility is wrong!");
            }
        }
    }
}

[Serializable]
public class LevelData
{
    public string Name;
    public int Width;
    public int Height;
    public string[] Palette;
    public string[] Data;
}