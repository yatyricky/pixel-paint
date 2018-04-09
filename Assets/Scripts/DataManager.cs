using System;
using System.Collections;
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
            }
        }

        Debug.Log(AllLevels.Count);
    }

    private void LoadSaveData()
    {
        // load saved files
        string dirPath = Path.Combine(Application.persistentDataPath, "SavedData");
        if (Directory.Exists(dirPath))
        {
            DirectoryInfo saveDir = new DirectoryInfo(dirPath);
            FileInfo[] saveInfo = saveDir.GetFiles("*.json");
            foreach (FileInfo f in saveInfo)
            {
                string key = f.Name.Split('.')[0];
                string json = File.ReadAllText(f.FullName);
                LevelData asset = JsonUtility.FromJson<LevelData>(json);
                AllSaves.Add(asset.Name, new LevelAsset(asset));
            }
        }
        Debug.Log(AllSaves.Count);
    }

    internal void UpdateSavedData(string key, LevelData data)
    {
        if (AllSaves.ContainsKey(key))
        {
            AllSaves[key] = new LevelAsset(data);
        }
        else
        {
            AllSaves.Add(key, new LevelAsset(data));
        }
    }
}
