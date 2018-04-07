using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class HallScene : MonoBehaviour
{
    public GameObject LevelEntrancePrefab;
    public TwoColumnLayout TrendingViewObjects;

    // Use this for initialization
    void Start()
    {
        LoadLevelData();
    }

    private void LoadLevelData()
    {
        // load saved files
        string savePath = Path.Combine(Application.streamingAssetsPath, "SavedData");
        DirectoryInfo saveDir = new DirectoryInfo(savePath);
        FileInfo[] saveInfo = saveDir.GetFiles("*.json");
        Dictionary<string, LevelAsset> saves = new Dictionary<string, LevelAsset>();
        foreach (FileInfo f in saveInfo)
        {
            string json = File.ReadAllText(f.FullName);
            LevelAsset save = JsonUtility.FromJson<LevelAsset>(json);
            saves.Add(f.Name, save);
        }

        // load level info
        string path = Path.Combine(Application.streamingAssetsPath, "LevelData");
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.json");
        foreach (FileInfo f in info)
        {
            string json = File.ReadAllText(f.FullName);
            LevelAsset level = JsonUtility.FromJson<LevelAsset>(json);
            GameObject go = Instantiate(LevelEntrancePrefab);
            TrendingViewObjects.AddChild(go);
            LevelAsset save = null;
            saves.TryGetValue(f.Name, out save);
            go.GetComponent<LevelEntrance>().SetData(level, f.Name, save);
        }
    }

}
