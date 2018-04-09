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
        LayoutChildren();
    }

    private void LayoutChildren()
    {
        LevelEntrance[] children = GetComponentsInChildren<LevelEntrance>();
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (children.Length + 1) / 2 * 424 + 14);
        for (int i = 0; i < children.Length; i ++)
        {
            int row = i / 2;
            int col = i % 2;
            Transform cur = children[i].gameObject.transform;
            Vector3 pos = new Vector3(col * 353 + 14, - 14 - row * 424, -0.1f);
            cur.localPosition = pos;
        }
    }

}
