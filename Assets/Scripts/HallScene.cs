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
