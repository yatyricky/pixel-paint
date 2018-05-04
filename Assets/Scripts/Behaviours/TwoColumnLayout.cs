using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwoColumnLayout : MonoBehaviour
{
    public string Name;

    private Dictionary<string, GameObject> LevelEntrances;

    private void Awake()
    {
        LevelEntrances = new Dictionary<string, GameObject>();
    }

    void Start()
    {
        LayoutChildren();
    }

    public bool HasLoad(string key)
    {
        return LevelEntrances.ContainsKey(key);
    }

    public void AddChild(GameObject obj, string key)
    {
        LevelEntrances.Add(key, obj);
        obj.transform.SetParent(gameObject.transform);
        if (Configs.SHOULD_CHANGE_WIDTH)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Configs.LEVEL_WIDTH, Configs.LEVEL_HEIGHT);
        }
        LayoutChildren();
    }

    private void LayoutChildren()
    {
        // resize container
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Mathf.Floor((LevelEntrances.Count + 1) / 2f) * (Configs.LEVEL_HEIGHT + Configs.LEVEL_MARGIN) + Configs.LEVEL_MARGIN);

        // sort
        List<GameObject> sorted = new List<GameObject>();
        List<string> sortData = DataManager.Instance.SortLoved;
        if (Name.Equals("Trend"))
        {
            sortData = DataManager.Instance.SortTrend;
        }
        List<string> inScene = LevelEntrances.Keys.ToList();
        for (int i = 0; i < sortData.Count; i ++)
        {
            string current = sortData.ElementAt(i);
            GameObject obj;
            if (LevelEntrances.TryGetValue(current, out obj))
            {
                sorted.Add(obj);
                inScene.Remove(current);
            }
            else
            {
            }
        }
        for (int i = 0; i < inScene.Count; i ++)
        {
            string current = inScene.ElementAt(i);
            GameObject obj;
            if (LevelEntrances.TryGetValue(current, out obj))
            {
                sorted.Insert(0, obj);
                sortData.Insert(0, current);
            }
            else
            {
                Debug.LogError("LayoutChildren:76: " + current);
            }
        }

        // layout children
        for (int i = 0; i < sorted.Count; i ++)
        {
            int row = i / 2;
            int col = i % 2;
            Transform cur = sorted.ElementAt(i).transform;
            Vector3 pos = new Vector3(col * (Configs.LEVEL_WIDTH + Configs.LEVEL_MARGIN) + Configs.LEVEL_MARGIN, 0f - Configs.LEVEL_MARGIN - row * (Configs.LEVEL_HEIGHT + Configs.LEVEL_MARGIN), -0.1f);
            cur.localPosition = pos;
        }

        // commit sort file async
        StartCoroutine(CommitSortFile(sortData));
    }

    private IEnumerator CommitSortFile(List<string> data)
    {
        yield return null;
        DataManager.Instance.CommitSort(Name, data);
    }
}
