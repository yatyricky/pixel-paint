using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class HallScene : MonoBehaviour
{
    public GameObject LevelEntrancePrefab;
    public TwoColumnLayout TrendingViewObjects;
    public TwoColumnLayout FavoriteViewObjects;
    public GameObject MorePage;
    public RectTransform Canvas;
    public BetterToggle TrendingToggle;

    private float CanvasTargetPos = 0 - Configs.SCREEN_WIDTH * 0.5f;
    private Vector3 velocity = Vector3.zero;

    private static HallScene self;
    private static Queue<Action> ReceivedActions = new Queue<Action>();

    private void Awake()
    {
        self = this;
    }

    private void Update()
    {
        if (Mathf.Abs(CanvasTargetPos - Canvas.position.x) > Configs.BIG_EPSILON)
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
                LevelAsset levelData;
                if (DataManager.Instance.AllLevels.TryGetValue(entry.Key, out levelData))
                {
                    if (!self.FavoriteViewObjects.HasLoad(entry.Key))
                    {
                        GameObject go = Instantiate(self.LevelEntrancePrefab);
                        self.FavoriteViewObjects.AddChild(go, entry.Key);
                        go.GetComponent<LevelEntrance>().SetData(levelData, entry.Value);
                    }
                }
                else
                {
                    Debug.Log("has save without level");
                }
            }
            // fill all levels
            foreach (KeyValuePair<string, LevelAsset> entry in DataManager.Instance.AllLevels)
            {
                if (!DataManager.Instance.AllSaves.ContainsKey(entry.Key))
                {
                    if (!self.TrendingViewObjects.HasLoad(entry.Key))
                    {
                        GameObject go = Instantiate(self.LevelEntrancePrefab);
                        self.TrendingViewObjects.AddChild(go, entry.Key);
                        go.GetComponent<LevelEntrance>().SetData(entry.Value, null);
                    }
                }
            }
            if (DataManager.Instance.AllSaves.Count == 0)
            {
                self.TrendingToggle.OnClicked();
                self.OnClickedTrending();
            }
            self.TrendingViewObjects.transform.parent.GetComponent<TrendingContents>().CompleteLoad();
        });
    }

    private void CanvasSwitchPage(int page)
    {
        CanvasTargetPos = Configs.SCREEN_WIDTH / 2f - page * Configs.SCREEN_WIDTH;
    }

    public void OnClickedFavorite()
    {
        CanvasSwitchPage(1);
        MorePage.SetActive(false);
    }

    public void OnClickedTrending()
    {
        CanvasSwitchPage(0);
        MorePage.SetActive(false);
    }

    public void OnClickedMore()
    {
        MorePage.SetActive(true);
    }

}
