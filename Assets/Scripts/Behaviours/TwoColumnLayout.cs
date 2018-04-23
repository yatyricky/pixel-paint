using System.Collections.Generic;
using UnityEngine;

public class TwoColumnLayout : MonoBehaviour
{
    private List<string> Keys;

    private void Awake()
    {
        Keys = new List<string>();
    }

    void Start()
    {
        LayoutChildren();
    }

    public bool HasLoad(string key)
    {
        return Keys.Contains(key);
    }

    public void AddChild(GameObject obj, string key)
    {
        Keys.Add(key);
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
        LevelEntrance[] children = GetComponentsInChildren<LevelEntrance>();
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Mathf.Floor((children.Length + 1) / 2f) * (Configs.LEVEL_HEIGHT + Configs.LEVEL_MARGIN) + Configs.LEVEL_MARGIN);
        for (int i = 0; i < children.Length; i ++)
        {
            int row = i / 2;
            int col = i % 2;
            Transform cur = children[i].gameObject.transform;
            Vector3 pos = new Vector3(col * (Configs.LEVEL_WIDTH + Configs.LEVEL_MARGIN) + Configs.LEVEL_MARGIN, 0f - Configs.LEVEL_MARGIN - row * (Configs.LEVEL_HEIGHT + Configs.LEVEL_MARGIN), -0.1f);
            cur.localPosition = pos;
        }
    }

}
