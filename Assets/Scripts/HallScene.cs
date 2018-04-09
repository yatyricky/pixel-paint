using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class HallScene : MonoBehaviour
{
    public GameObject LevelEntrancePrefab;
    public TwoColumnLayout TrendingViewObjects;
    public TwoColumnLayout FavoriteViewObjects;
    public RectTransform Canvas;

    private float CanvasTargetPos = -360f;
    private Vector3 velocity = Vector3.zero;

    private static HallScene self;
    private static Queue<Action> ReceivedActions = new Queue<Action>();

    private void Awake()
    {
        self = this;
    }

    // Use this for initialization
    void Start()
    {
        //LoadLevelData();
        //LoadFavData();
    }

    private void Update()
    {
        if (Mathf.Abs(CanvasTargetPos - Canvas.position.x) > float.Epsilon)
        {
            Canvas.position = Vector3.SmoothDamp(Canvas.position, new Vector3(CanvasTargetPos, Canvas.position.y, 0), ref velocity, 0.1f);
        }
        while (ReceivedActions.Count > 0)
        {
            ReceivedActions.Dequeue()();
        }
    }

    private void LoadLevelData()
    {
        // Do not load played levels
        string savePath = Path.Combine(Application.streamingAssetsPath, "SavedData");
        DirectoryInfo saveDir = new DirectoryInfo(savePath);
        FileInfo[] saveInfo = saveDir.GetFiles("*.json");
        string[] files = new string[saveInfo.Length];
        for (int i = 0; i < saveInfo.Length; i ++)
        {
            files[i] = saveInfo[i].Name;
        }

        // load level info
        string path = Path.Combine(Application.streamingAssetsPath, "LevelData");
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.json");
        foreach (FileInfo f in info)
        {
            if (Array.IndexOf<string>(files, f.Name) == -1)
            {
                string json = File.ReadAllText(f.FullName);
                LevelAsset level = new LevelAsset(JsonUtility.FromJson<LevelData>(json));
                GameObject go = Instantiate(LevelEntrancePrefab);
                TrendingViewObjects.AddChild(go);
                go.GetComponent<LevelEntrance>().SetData(level, null);
            }
        }
    }

    private void LoadFavData()
    {
        // load saved files
        string savePath = Path.Combine(Application.streamingAssetsPath, "SavedData");
        DirectoryInfo saveDir = new DirectoryInfo(savePath);
        FileInfo[] saveInfo = saveDir.GetFiles("*.json");
        Dictionary<string, LevelAsset> saves = new Dictionary<string, LevelAsset>();
        foreach (FileInfo f in saveInfo)
        {
            string json = File.ReadAllText(f.FullName);
            LevelAsset save = new LevelAsset(JsonUtility.FromJson<LevelData>(json));
            saves.Add(f.Name, save);
        }

        // load level info
        string path = Path.Combine(Application.streamingAssetsPath, "LevelData");
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.json");
        foreach (FileInfo f in info)
        {
            LevelAsset save = null;
            if (saves.TryGetValue(f.Name, out save))
            {
                string json = File.ReadAllText(f.FullName);
                LevelAsset level = new LevelAsset(JsonUtility.FromJson<LevelData>(json));
                GameObject go = Instantiate(LevelEntrancePrefab);
                FavoriteViewObjects.AddChild(go);
                go.GetComponent<LevelEntrance>().SetData(level, save);
            }
        }
    }

    public static void DispatchRenderLevels()
    {
        ReceivedActions.Enqueue(() =>
        {
            // fill saves
            foreach (KeyValuePair<string, LevelAsset> entry in DataManager.Instance.AllSaves)
            {
                GameObject go = Instantiate(self.LevelEntrancePrefab);
                self.FavoriteViewObjects.AddChild(go);
                LevelAsset levelData;
                if (DataManager.Instance.AllLevels.TryGetValue(entry.Key, out levelData))
                {
                    go.GetComponent<LevelEntrance>().SetData(levelData, entry.Value);
                }
                else
                {
                    throw new Exception("has save without level");
                }
            }
            // fill all levels
            Debug.Log("Trying to load all levles");
            foreach (KeyValuePair<string, LevelAsset> entry in DataManager.Instance.AllLevels)
            {
                if (!DataManager.Instance.AllSaves.ContainsKey(entry.Key))
                {
                    GameObject go = Instantiate(self.LevelEntrancePrefab);
                    self.TrendingViewObjects.AddChild(go);
                    go.GetComponent<LevelEntrance>().SetData(entry.Value, null);
                }
            }
        });
    }

    private void CanvasSwitchPage(int page)
    {
        CanvasTargetPos = Configs.DESIGN_WIDTH / 2f - page * Configs.DESIGN_WIDTH;
    }

    public void OnClickedFavorite()
    {
        CanvasSwitchPage(1);
    }

    public void OnClickedTrending()
    {
        CanvasSwitchPage(0);
    }

}
