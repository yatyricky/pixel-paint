using UnityEngine;
using System.Collections;

public class TrendingContents : MonoBehaviour
{
    public InlineLoader Loader;
    public TwoColumnLayout Levels;
    public float LoaderHeight;
    public float Threshold;

    private RectTransform mRect;
    private BoxCollider mCollider;
    private RectTransform levelsRect;

    private void Awake()
    {
        mRect = GetComponent<RectTransform>();
        mCollider = GetComponent<BoxCollider>();
        levelsRect = Levels.GetComponent<RectTransform>();
        Resize();
    }

    private void OnMouseUp()
    {
        if (transform.localPosition.y < Threshold)
        {
            if (!Loader.IsRunning())
            {
                Loader.InitiateSpin();
                DataManager.Instance.RequestUpdateLevels();
                Resize();
            }
        }
    }

    private void Resize()
    {
        float height;
        float margin;
        if (Loader.IsRunning())
        {
            height = levelsRect.sizeDelta.y + LoaderHeight;
            margin = 0f - LoaderHeight;
        }
        else
        {
            height = levelsRect.sizeDelta.y;
            margin = 0;
        }
        mRect.sizeDelta = new Vector2(0, height);
        Levels.transform.localPosition = new Vector3(0, margin, 0);
        mCollider.size = new Vector3(720f, height, 1f);
        mCollider.center = new Vector3(360f, 0f - height / 2f, 0f);
    }

    public void CompleteLoad()
    {
        Loader.Halt();
        Resize();
    }

}
