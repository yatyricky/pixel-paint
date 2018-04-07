#if UNITY_EDITOR
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
    private Color[] _data;
    private int _height;
    private int _width;

    // Use this for initialization
    public void Preview()
    {
        _distinctColors = new Dictionary<string, int>();
        _data = new Color[SourceImage.width * SourceImage.height];
        _width = SourceImage.width;
        _height = SourceImage.height;

        for (int y = 0; y < SourceImage.height; y ++)
        {
            for (int x = 0; x < SourceImage.width; x ++)
            {
                Color color = SourceImage.GetPixel(x, y);
                if (color.a == 0f)
                {
                    color = new Color(0, 0, 0, 0);
                }
                string colorHex = ColorUtility.ToHtmlStringRGBA(color);
                
                //Debug.Log(x+","+y+":"+ colorHex);
                Vector3Int pos = new Vector3Int(x, y, 0);
                Canvas.SetTileFlags(pos, TileFlags.None);
                Canvas.SetTile(pos, WhiteTile);
                Canvas.SetTileFlags(pos, TileFlags.None);
                float gray = color.grayscale;
                Canvas.SetColor(pos, color);

                int test;
                if (!_distinctColors.TryGetValue(colorHex, out test))
                {
                    //Debug.Log("New color " + colorHex);
                    _distinctColors.Add(colorHex, 1);
                }
                _data[y * SourceImage.width + x] = color;
            }
        }
    }

    public void Generate()
    {

        LevelAsset asset = new LevelAsset();
        asset.Height = _height;
        asset.Width = _width;
        List<Color> palette = new List<Color>();
        for (int i = 0; i < _distinctColors.Count; i ++)
        {
            string hex = "#" + _distinctColors.ElementAt(i).Key;
            //Debug.Log(hex);
            Color color;
            if (ColorUtility.TryParseHtmlString(hex, out color))
            {
                if (color.a != 0f)
                {
                    palette.Add(color);
                }
            }
            else
            {
                throw new System.Exception("ColorUtility is wrong!");
            }
        }
        asset.Palette = palette.ToArray<Color>();
        asset.Data = _data;

        string json = JsonUtility.ToJson(asset);
        File.WriteAllText(Path.Combine(Path.Combine(Application.streamingAssetsPath, "LevelData"), SourceImage.name + ".json"), json);
    }

}
#endif
