using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public TextAsset[] InternalLevels;

    public Dictionary<string, LevelAsset> AllLevels;
    public Dictionary<string, LevelAsset> AllSaves;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        AllLevels = new Dictionary<string, LevelAsset>();
        AllSaves = new Dictionary<string, LevelAsset>();
    }

    private void Start()
    {
        LoadLevelData();
        LoadSaveData();
        HallScene.DispatchRenderLevels();
    }

    private void LoadLevelData()
    {
        // load streamed level first
        //string path = Path.Combine(Application.streamingAssetsPath, "LevelData");
        //DirectoryInfo dir = new DirectoryInfo(path);
        //FileInfo[] info = dir.GetFiles("*.json");
        //foreach (FileInfo f in info)
        //{
        //    string key = f.Name.Split('.')[0];
        //    string json = File.ReadAllText(f.FullName);
        //    LevelData asset = JsonUtility.FromJson<LevelData>(json);
        //    AllLevels.Add(asset.Name, new LevelAsset(asset));
        //}

        // Load internal levels
        for (int i = 0; i < InternalLevels.Length; i++)
        {
            if (!AllLevels.ContainsKey(InternalLevels[i].name))
            {
                LevelData asset = JsonUtility.FromJson<LevelData>(InternalLevels[i].text);
                AllLevels.Add(asset.Name, new LevelAsset(asset));
                Debug.Log("Level name = " + asset.Name);
            }
        }
    }

    private void LoadSaveData()
    {
        // load saved files
        //string savePath = Path.Combine(Application.streamingAssetsPath, "SavedData");
        //DirectoryInfo saveDir = new DirectoryInfo(savePath);
        //FileInfo[] saveInfo = saveDir.GetFiles("*.json");
        //foreach (FileInfo f in saveInfo)
        //{
        //    string key = f.Name.Split('.')[0];
        //    string json = File.ReadAllText(f.FullName);
        //    LevelData asset = JsonUtility.FromJson<LevelData>(json);
        //    AllLevels.Add(asset.Name, new LevelAsset(asset));
        //}
        //Debug.Log("Load save data done");
    }

}
