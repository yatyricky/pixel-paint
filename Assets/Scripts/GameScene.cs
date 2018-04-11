using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

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
    [HideInInspector] public LevelAsset Save;
    [HideInInspector] public bool touched = false;

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
        if (Save == null)
        {
            Save = new LevelAsset(Level, true);
        }
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
            Canvas.SetColor(pos, Save.Data[i]);

            MarkerOverlay.SetTileFlags(pos, TileFlags.None);
            if (color.a != 0f)
            {
                MarkerOverlay.SetTile(pos, Markers[Array.IndexOf(Level.Palette, color)]);
            }
        }

        // zoom camera properly
        int heightPixels = (int)(Level.Height / Configs.ART_IN_WINDOW_RATIO);
        int widthPixels = (int)(Level.Width / Configs.ART_IN_WINDOW_RATIO);
        float camSize = Configs.ZOOM_MIN;
        if (heightPixels / widthPixels > Configs.WINDOW_RATIO)
        {
            // too high
            camSize = heightPixels / Configs.WINDOW_RATIO / Configs.PIXEL_WIDTH_CAM_RATIO;
        }
        else
        {
            // too wide
            camSize = widthPixels / Configs.PIXEL_WIDTH_CAM_RATIO;
        }
        GameController.ZoomTo(camSize);
    }

    internal void HighlightInCanvas(Color color)
    {
        for (int i = 0; i < Save.Data.Length; i++)
        {
            int y = i / Save.Width;
            int x = i - y * Save.Width;
            Vector3Int pos = new Vector3Int(x, y, 0);

            if (IsClickable(pos))
            {
                Canvas.SetTileFlags(pos, TileFlags.None);
                if (color.Equals(Level.Data[i]))
                {
                    Canvas.SetColor(pos, new Color(Save.Data[i].r * Configs.HIGHLIGHT_CANVAS_RATIO, Save.Data[i].g * Configs.HIGHLIGHT_CANVAS_RATIO, Save.Data[i].b * Configs.HIGHLIGHT_CANVAS_RATIO, Save.Data[i].a));
                }
                else
                {
                    Canvas.SetColor(pos, Save.Data[i]);
                }
            }
        }
    }

    internal void Fill(Vector3Int pos, Color color)
    {
        if (IsClickable(pos))
        {
            Save.Data[pos.y * Save.Width + pos.x] = color;
            Canvas.SetTileFlags(pos, TileFlags.None);
            Canvas.SetColor(pos, color);

            MarkerOverlay.SetTileFlags(pos, TileFlags.None);
            MarkerOverlay.SetColor(pos, MarkerShouldBe(pos));
        }
    }

    private void InitPallete()
    {
        ColorPicker firstPicker = null;
        float width = Level.Palette.Length * Configs.PALETTE_WIDTH;
        PallateContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0);
        BoxCollider collider = PallateContainer.GetComponent<BoxCollider>();
        collider.size = new Vector3(width, collider.size.y, collider.size.z);
        collider.center = new Vector3(width / 2f, collider.center.y, collider.center.z);
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

    internal bool IsClickable(Vector3Int position)
    {
        // out of bounds
        if (position.x < 0 || position.x >= Level.Width || position.y < 0 || position.y >= Level.Height)
        {
            return false;
        }
        // already true
        Color currentColor = Canvas.GetColor(position);
        Color trueColor = Level.Data[position.y * Level.Width + position.x];
        if (currentColor.Equals(trueColor))
        {
            return false;
        }
        // transparent
        if (trueColor.a == 0f)
        {
            return false;
        }
        return true;
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("Hall");

        if (touched == true)
        {
            SaveGame();
        }
        HallScene.DispatchRenderLevels();
    }

    private void SaveGame()
    {
        string json = JsonUtility.ToJson(Save.ToLevelData());
        string dirPath = Path.Combine(Application.persistentDataPath, "SavedData");
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllText(Path.Combine(dirPath, Save.Name + ".json"), json);
        DataManager.Instance.UpdateSavedData(Level.Name, Save);
    }

    public static void DispatchSetGameData(LevelAsset level, LevelAsset save)
    {
        ReceivedActions.Enqueue(() =>
        {
            self.Level = level;
            self.Save = save;
            self.InitWorld();
            self.InitPallete();
        });
    }

    private Color MarkerShouldBe(Vector3Int pos)
    {
        Color color;
        int index = pos.y * Level.Width + pos.x;
        // correct fill
        if (Canvas.GetColor(pos).Equals(Level.Data[index]))
        {
            color = new Color(0, 0, 0, 0);
        }
        else
        {
            // too far away
            if (GameCamera.orthographicSize > Configs.ZOOM_HIDE_MARKER)
            {
                color = new Color(0, 0, 0, 0);
            }
            else
            {
                float greyColor = 1 - (GameCamera.orthographicSize - Configs.ZOOM_FADE_MARKER) / (Configs.ZOOM_HIDE_MARKER - Configs.ZOOM_FADE_MARKER);
                if (greyColor > 1)
                {
                    greyColor = 1;
                }
                
                float black = 0f;
                if (Save.Data[index].grayscale < Configs.FLIP_MARKER)
                {
                    black = 1f;
                }
                color = new Color(black, black, black, greyColor);
            }
        }
        return color;
    }

    internal void UpdateMarkers()
    {
        for (int i = 0; i < Save.Data.Length; i++)
        {
            int y = i / Save.Width;
            int x = i - y * Save.Width;
            Vector3Int pos = new Vector3Int(x, y, 0);

            MarkerOverlay.SetTileFlags(pos, TileFlags.None);
            MarkerOverlay.SetColor(pos, MarkerShouldBe(pos));
        }
    }
}