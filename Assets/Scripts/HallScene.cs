using UnityEngine;
using System.IO;

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
        string path = Path.Combine(Application.streamingAssetsPath, "LevelData");
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.json");
        foreach (FileInfo f in info)
        {
            string json = File.ReadAllText(f.FullName);
            LevelAsset level = JsonUtility.FromJson<LevelAsset>(json);
            GameObject go = Instantiate(LevelEntrancePrefab);
            TrendingViewObjects.AddChild(go);
            go.GetComponent<LevelEntrance>().SetData(level);
        }
    }

}
