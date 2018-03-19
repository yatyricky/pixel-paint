using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System;

public class LevelLoader : MonoBehaviour
{
    public Camera GameCamera;
    public GameObject TilemapObj;
    public TileBase WhiteTile;
    public GameObject PallateContainer;
    public GameObject ColorPickerPrefab;
    public Tilemap Canvas;
    public Tilemap MarkerOverlay;
    public TileBase[] Markers;
    public Player GameController;

    void Start()
    {
        InitPallete(GameController.CurrentLevel);
        InitWorld(GameController.CurrentLevel);
    }

    private void InitWorld(LevelAsset asset)
    {
        GameCamera.transform.position = new Vector3(asset.Width / 2, asset.Height / 2, -20);
        for (int i = 0; i < asset.Data.Length; i ++)
        {
            int x = i / asset.Height;
            int y = i - x * asset.Height;
            Color color = asset.Data[i];

            //Debug.Log(x+","+y+":"+ colorHex);
            Vector3Int pos = new Vector3Int(x, y, 0);
            Canvas.SetTileFlags(pos, TileFlags.None);
            Canvas.SetTile(pos, WhiteTile);
            Canvas.SetTileFlags(pos, TileFlags.None);
            float gray = color.grayscale;
            Canvas.SetColor(pos, new Color(gray, gray, gray, color.a));

            MarkerOverlay.SetTileFlags(pos, TileFlags.None);
            MarkerOverlay.SetTile(pos, Markers[Array.IndexOf(asset.Palette, color)]);
        }
    }

    private void InitPallete(LevelAsset asset)
    {
        ColorPicker firstPicker = null;
        PallateContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(asset.Palette.Length * 144, 0);
        for (int i = 0; i < asset.Palette.Length; i ++)
        {
            GameObject go = Instantiate(ColorPickerPrefab);
            ColorPicker colorPicker = go.GetComponent<ColorPicker>();
            go.transform.parent = PallateContainer.transform;
            colorPicker.Init(asset.Palette[i], i + 1);
            if (firstPicker == null)
            {
                firstPicker = colorPicker;
            }
        }
        firstPicker.ColorPicked();
    }
}