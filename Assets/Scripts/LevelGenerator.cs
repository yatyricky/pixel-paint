using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class LevelGenerator : MonoBehaviour
{
    [Header("Setup")]
    public Tilemap Canvas;
    public TileBase WhiteTile;

    [Header("Edit")]
    public Texture2D SourceImage;

    private Dictionary<Color, int> _distinctColors;
    private Color[] _data;
    private int _height;
    private int _width;

    // Use this for initialization
    public void Preview()
    {
        _distinctColors = new Dictionary<Color, int>();
        _data = new Color[SourceImage.width * SourceImage.height];
        _width = SourceImage.width;
        _height = SourceImage.height;
        for (int x = 0; x < SourceImage.width; x ++)
        {
            for (int y = 0; y < SourceImage.height; y ++)
            {
                Color color = SourceImage.GetPixel(x, y);
                Vector3Int pos = new Vector3Int(x, y, 0);
                Canvas.SetTileFlags(pos, TileFlags.None);
                Canvas.SetTile(pos, WhiteTile);
                Canvas.SetTileFlags(pos, TileFlags.None);
                float gray = color.grayscale;
                Canvas.SetColor(pos, new Color(gray, gray, gray, gray));

                int test;
                if (!_distinctColors.TryGetValue(color, out test))
                {
                    _distinctColors.Add(color, 1);
                }
                _data[x * SourceImage.height + y] = color;
            }
        }
    }

    public void Generate()
    {
        LevelAsset asset = new LevelAsset
        {
            Height = _height,
            Width = _width,
            Palette = _distinctColors.Keys.ToArray(),
            Data = _data,
            PaletteSize = _distinctColors.Keys.Count,
            DataSize = _data.Length
        };
        AssetDatabase.CreateAsset(asset, "Assets/Levels/" + SourceImage.name + ".asset");
    }

}
