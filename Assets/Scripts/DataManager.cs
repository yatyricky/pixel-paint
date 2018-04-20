using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance = null;

    public TextAsset[] InternalLevels;

    public Dictionary<string, LevelAsset> AllLevels;
    public Dictionary<string, LevelAsset> AllSaves;

    private bool interstitialAdCDRunning;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialization();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Initialization()
    {
        AllLevels = new Dictionary<string, LevelAsset>();
        AllSaves = new Dictionary<string, LevelAsset>();

        // Initialize the Google Mobile Ads SDK.
#if UNITY_ANDROID
        string appId = "ca-app-pub-7959728254107074~3791366270";
#elif UNITY_IOS
        string appId = "ca-app-pub-7959728254107074~6556473878";
#else
        string appId = "unexpected_platform";
#endif
        MobileAds.Initialize(appId);

        interstitialAdCDRunning = false;
        LoadLevelData();
        LoadSaveData();
        HallScene.DispatchRenderLevels();
    }

    private void LoadLevelData()
    {
        // load streamed level first
        string dirPath = Path.Combine(Application.persistentDataPath, "LevelData");
        if (Directory.Exists(dirPath))
        {
            DirectoryInfo levelDir = new DirectoryInfo(dirPath);
            FileInfo[] saveInfo = levelDir.GetFiles("*.json");
            foreach (FileInfo f in saveInfo)
            {
                string key = f.Name.Split('.')[0];
                string json = File.ReadAllText(f.FullName);
                LevelData asset = JsonUtility.FromJson<LevelData>(json);
                AllLevels.Add(asset.Name, new LevelAsset(asset));
            }
        }
        else
        {
            Directory.CreateDirectory(dirPath);
        }

        // Load internal levels
        for (int i = 0; i < InternalLevels.Length; i++)
        {
            if (!AllLevels.ContainsKey(InternalLevels[i].name))
            {
                LevelData asset = JsonUtility.FromJson<LevelData>(InternalLevels[i].text);
                AllLevels.Add(asset.Name, new LevelAsset(asset));
            }
        }

        // get updates
        RequestUpdateLevels();
    }

    public void RequestUpdateLevels()
    {
        string[] keys = new string[AllLevels.Keys.Count];
        AllLevels.Keys.CopyTo(keys, 0);
        StartCoroutine(DownloadNewLevels(string.Join(",", keys)));
    }

    private IEnumerator DownloadNewLevels(string files)
    {
        string request = Configs.SERVER + ":" + Configs.PORT + "/?secret=" + Configs.SECRET + "&list=" + files;
        WWW www = new WWW(request);
        yield return www;

        string dirPath = Application.persistentDataPath + "/LevelData";
        string zipPath = dirPath  + "/bundle.zip";
        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }
        File.WriteAllBytes(zipPath, www.bytes);
        ZipUtil.Unzip(zipPath, dirPath);
        File.Delete(zipPath);
        UpdateLevelData();
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
    }

    internal void UpdateSavedData(string key, LevelAsset data)
    {
        if (AllSaves.ContainsKey(key))
        {
            AllSaves[key] = data;
        }
        else
        {
            AllSaves.Add(key, data);
        }
    }

    internal void UpdateLevelData()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, "LevelData");
        if (Directory.Exists(dirPath))
        {
            DirectoryInfo levelDir = new DirectoryInfo(dirPath);
            FileInfo[] saveInfo = levelDir.GetFiles("*.json");
            foreach (FileInfo f in saveInfo)
            {
                string key = f.Name.Split('.')[0];
                if (!AllLevels.ContainsKey(key))
                {
                    string json = File.ReadAllText(f.FullName);
                    LevelData asset = JsonUtility.FromJson<LevelData>(json);
                    AllLevels.Add(asset.Name, new LevelAsset(asset));
                }
            }
        }
        HallScene.DispatchRenderLevels();
    }

    public bool CanDisplayInterstitialAds()
    {
        if (interstitialAdCDRunning)
        {
            return false;
        }
        else
        {
            StartCoroutine(InterstitialAdCoolDownRunner());
            return true;
        }
    }

    private IEnumerator InterstitialAdCoolDownRunner()
    {
        interstitialAdCDRunning = true;
        yield return new WaitForSeconds(Configs.INTERSTITIAL_AD_CD);
        interstitialAdCDRunning = false;
    }
}
