using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using GoogleMobileAds.Api;

public class GameScene : MonoBehaviour
{
    public Camera GameCamera;
    public GameObject TilemapObj;
    public TileBase WhiteTile;
    public GameObject PallateContainer;
    public GameObject ColorPickerPrefab;
    public Tilemap Canvas;
    public Tilemap MarkerOverlay;
    public Player GameController;
    public TileBase[] Markers;
    [HideInInspector] public LevelAsset Level;
    [HideInInspector] public LevelAsset Save;
    [HideInInspector] public bool touched = false;

    [Header("UI")]
    public GameObject BackButton;
    public GameObject MuteButton;
    public float BackButtonTopMargin;

    private List<ColorPicker> Pickers;
    private BannerView bannerView;
    InterstitialAd interstitial;

    private static GameScene self;
    private static Queue<Action> ReceivedActions = new Queue<Action>();

    private void Awake()
    {
        self = this;
    }

    private void Start()
    {
        RequestBanner();
        if (DataManager.Instance.CanDisplayInterstitialAds())
        {
            RequestInterstitial();
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (ReceivedActions.Count > 0)
        {
            ReceivedActions.Dequeue()();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackClicked();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        SaveGame();
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // test
        //string adUnitId = "ca-app-pub-7959728254107074/9782059553"; // dist
#elif UNITY_IOS
        //string adUnitId = "ca-app-pub-3940256099942544/2934735716"; // test
        string adUnitId = "ca-app-pub-7959728254107074/7566959755"; // dist
#else
        string adUnitId = "unexpected_platform";
#endif
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice("4CFBEFA67D908C145471D121713E0390") // test device
            .Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    private void HandleOnAdLoaded(object sender, EventArgs e)
    {
        // move down back button
        DispatchReposYBackButton(0f - (bannerView.GetHeightInPixels() / Screen.height + BackButtonTopMargin / Configs.DESIGN_HEIGHT) * Configs.DESIGN_HEIGHT);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; // test
        //string adUnitId = "ca-app-pub-7959728254107074/9398916175"; // dist
#elif UNITY_IOS
        //string adUnitId = "ca-app-pub-3940256099942544/4411468910"; // test
        string adUnitId = "ca-app-pub-7959728254107074/8113754665"; // dist
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);
        // Called when the ad is closed.
        interstitial.OnAdClosed += HandleOnIntAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice("4CFBEFA67D908C145471D121713E0390") // test device
            .Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    private void HandleOnIntAdClosed(object sender, EventArgs e)
    {
        interstitial.Destroy();
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
            // fill clicked cell with picked color
            Save.Data[pos.y * Save.Width + pos.x] = color;
            Canvas.SetTileFlags(pos, TileFlags.None);
            Canvas.SetColor(pos, color);

            // update marker based on filled color grey scale
            MarkerOverlay.SetTileFlags(pos, TileFlags.None);
            MarkerOverlay.SetColor(pos, MarkerShouldBe(pos));

            // set marker to completed state
            UpdatePicker();

            DataManager.Instance.AudioManager.PlayOneKey();

            touched = true;
        }
    }

    private void UpdatePicker()
    {
        bool completed = true;
        for (int i = 0; i < Level.Data.Length && completed; i ++)
        {
            if (Level.Data[i].Equals(GameController.CurrentColor.SelColor))
            {
                if (!Level.Data[i].Equals(Save.Data[i]))
                {
                    completed = false;
                }
            }
        }
        if (completed)
        {
            GameController.CurrentColor.SetComplete();
        }
    }

    private void UpdatePickerAll()
    {
        bool[] completed = new bool[Pickers.Count];
        for (int i = 0; i < completed.Length; i ++)
        {
            completed[i] = true;
        }
        Dictionary<string, int> dict = new Dictionary<string, int>();
        for (int i = 0; i < Pickers.Count; i ++)
        {
            dict.Add(ColorUtility.ToHtmlStringRGBA(Pickers.ElementAt(i).SelColor), i);
        }
        for (int i = 0; i < Level.Data.Length; i++)
        {
            int index;
            if (dict.TryGetValue(ColorUtility.ToHtmlStringRGBA(Level.Data[i]), out index))
            {
                if (!Level.Data[i].Equals(Save.Data[i]) && completed[index])
                {
                    completed[index] = false;
                }
            }
        }
        for (int i = 0; i < completed.Length; i++)
        {
            if (completed[i])
            {
                Pickers.ElementAt(i).SetComplete();
            }
        }
    }

    private void InitPallete()
    {
        Pickers = new List<ColorPicker>();
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
            Pickers.Add(colorPicker);
        }
        firstPicker.ColorPicked();

        // update all pickers
        UpdatePickerAll();
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

        // ads behaviour when exit
        bannerView.Destroy();
        if (interstitial != null)
        {
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
            }
            else
            {
                interstitial.Destroy();
            }
        }
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

    public static void DispatchReposYBackButton(float y)
    {
        ReceivedActions.Enqueue(() =>
        {
            RectTransform rt = self.BackButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
            rt = self.MuteButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
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
 