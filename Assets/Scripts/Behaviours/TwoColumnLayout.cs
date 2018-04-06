using UnityEngine;

[ExecuteInEditMode]
public class TwoColumnLayout : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        LayoutChildren();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddChild(GameObject obj)
    {
        obj.transform.SetParent(gameObject.transform);
        LayoutChildren();
    }

    private void LayoutChildren()
    {
        LevelEntrance[] children = GetComponentsInChildren<LevelEntrance>();
        for (int i = 0; i < children.Length; i ++)
        {
            int row = i / 2;
            int col = i % 2;
            Transform cur = children[i].gameObject.transform;
            Vector3 pos = new Vector3(col * 353 + 14 - 360, 640 - row * 424 - 14, -0.1f);
            cur.position = pos;
        }
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (children.Length + 1) / 2 * 424 + 14);
    }

}
