﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class LevelGenerator : MonoBehaviour
{
    [Header("Setup")]
    public Tilemap Canvas;
    public TileBase WhiteTile;

    [Header("Edit")]
    public Texture2D SourceImage;

    private Dictionary<string, int> _distinctColors;
    private string[] _data;
    private string _name;
    private int _height;
    private int _width;

    // Use this for initialization
    public void Preview()
    {
        _distinctColors = new Dictionary<string, int>();
        _data = new string[SourceImage.width * SourceImage.height];
        _width = SourceImage.width;
        _height = SourceImage.height;
        _name = SourceImage.name;

        for (int y = 0; y < SourceImage.height; y++)
        {
            for (int x = 0; x < SourceImage.width; x++)
            {
                Color color = SourceImage.GetPixel(x, y);
                if (color.a == 0f)
                {
                    color = new Color(0, 0, 0, 0);
                }
                string colorHex = ColorUtility.ToHtmlStringRGBA(color);

                Vector3Int pos = new Vector3Int(x, y, 0);
                Canvas.SetTileFlags(pos, TileFlags.None);
                Canvas.SetTile(pos, WhiteTile);
                Canvas.SetTileFlags(pos, TileFlags.None);
                float gray = color.grayscale;
                Canvas.SetColor(pos, color);

                int test;
                if (!_distinctColors.TryGetValue(colorHex, out test))
                {
                    _distinctColors.Add(colorHex, 1);
                }
                _data[y * SourceImage.width + x] = colorHex;
            }
        }
        Debug.Log("Image: " + _name);
        Debug.Log("Width: " + _width);
        Debug.Log("Height: " + _height);
        Debug.Log("Colors: " + _distinctColors.Count);
    }

    public void Generate()
    {
        LevelData asset = new LevelData();
        asset.Height = _height;
        asset.Width = _width;
        asset.Name = _name;
        List<string> palette = new List<string>();
        for (int i = 0; i < _distinctColors.Count; i++)
        {
            string hex = _distinctColors.ElementAt(i).Key;
            if (!hex.Substring(6).Equals("00"))
            {
                palette.Add(hex);
            }
        }
        asset.Palette = palette.ToArray<string>();
        asset.Data = _data;

        string json = JsonUtility.ToJson(asset);
        File.WriteAllText(Path.Combine(Application.dataPath + "/RawLevel/Levels", SourceImage.name + ".json"), json);
        Debug.Log("Generate success: " + _name);
    }

}
#endif
