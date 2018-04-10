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

    public LevelAsset(LevelAsset asset, bool toGray)
    {
        Name = asset.Name;
        Width = asset.Width;
        Height = asset.Height;
        Palette = new Color[asset.Palette.Length];
        for (int i = 0; i < Palette.Length; i++)
        {
            Palette[i] = new Color(asset.Palette[i].r, asset.Palette[i].g, asset.Palette[i].b, asset.Palette[i].a);
        }
        Data = new Color[asset.Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            if (toGray)
            {
                Data[i] = Utils.ConvertGreyscale(asset.Data[i]);
            }
            else
            {
                Data[i] = new Color(asset.Data[i].r, asset.Data[i].g, asset.Data[i].b, asset.Data[i].a);
            }
        }
    }

    public LevelData ToLevelData()
    {
        LevelData data = new LevelData();
        data.Name = Name;
        data.Height = Height;
        data.Width = Width;
        data.Palette = new string[Palette.Length];
        for (int i = 0; i < Palette.Length; i++)
        {
            data.Palette[i] = ColorUtility.ToHtmlStringRGBA(Palette[i]);
        }
        data.Data = new string[Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            data.Data[i] = ColorUtility.ToHtmlStringRGBA(Data[i]);
        }
        return data;
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