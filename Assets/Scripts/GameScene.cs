using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
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
    [HideInInspector] public LevelAsset Level;

    private static GameScene self;
    private static Queue<Action> ReceivedActions = new Queue<Action>();

    private void Awake()
    {
        self = this;
    }

    // Update is called once per frame
    void Update()
    {
        while (ReceivedActions.Count > 0)
        {
            ReceivedActions.Dequeue()();
        }
    }

    private void InitWorld()
    {
        GameCamera.transform.position = new Vector3(Level.Width / 2, Level.Height / 2, -20);
        for (int i = 0; i < Level.Data.Length; i ++)
        {
            int y = i / Level.Width;
            int x = i - y * Level.Width;
            Color color = Level.Data[i];

            //Debug.Log(x+","+y+":"+ colorHex);
            Vector3Int pos = new Vector3Int(x, y, 0);
            Canvas.SetTileFlags(pos, TileFlags.None);
            Canvas.SetTile(pos, WhiteTile);
            Canvas.SetTileFlags(pos, TileFlags.None);
            float gray = color.grayscale;
            Canvas.SetColor(pos, new Color(gray, gray, gray, color.a));

            MarkerOverlay.SetTileFlags(pos, TileFlags.None);
            MarkerOverlay.SetTile(pos, Markers[Array.IndexOf(Level.Palette, color)]);
        }
    }

    private void InitPallete()
    {
        ColorPicker firstPicker = null;
        PallateContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(Level.Palette.Length * 144, 0);
        for (int i = 0; i < Level.Palette.Length; i ++)
        {
            GameObject go = Instantiate(ColorPickerPrefab);
            ColorPicker colorPicker = go.GetComponent<ColorPicker>();
            go.transform.SetParent(PallateContainer.transform);
            colorPicker.Init(Level.Palette[i], i + 1);
            if (firstPicker == null)
            {
                firstPicker = colorPicker;
            }
        }
        firstPicker.ColorPicked();
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("Hall");
    }

    public static void DispatchSetGameData(LevelAsset level)
    {
        ReceivedActions.Enqueue(() =>
        {
            self.Level = level;
            self.InitPallete();
            self.InitWorld();
        });
    }

}