using UnityEngine;
using System.Collections;

public class TrendingContents : MonoBehaviour
{
    public InlineLoader Loader;
    public TwoColumnLayout Levels;
    public float Threshold;

    private float mLoaderHeight;
    private RectTransform mRect;
    private BoxCollider mCollider;
    private RectTransform levelsRect;
    private Vector3 velocity = Vector3.zero;
    private bool dragging;

    private void Awake()
    {
        mRect = GetComponent<RectTransform>();
        mCollider = GetComponent<BoxCollider>();
        mLoaderHeight = Loader.GetComponent<RectTransform>().sizeDelta.y;
        levelsRect = Levels.GetComponent<RectTransform>();
        Resize();
    }

    private void Update()
    {
        if (!Loader.IsRunning() && !dragging)
        {
            if (transform.localPosition.y - mLoaderHeight < 0f)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(0, mLoaderHeight, 0), ref velocity, 0.05f);
            }
        }
    }

    private void OnMouseDrag()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
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
        float height = levelsRect.sizeDelta.y + mLoaderHeight;
        mRect.sizeDelta = new Vector2(0, height);
        mCollider.size = new Vector3(Configs.DESIGN_WIDTH, height, 1f);
        mCollider.center = new Vector3(Configs.DESIGN_WIDTH / 2f, 0f - height / 2f, 0f);
    }

    public void CompleteLoad()
    {
        Loader.Halt();
        Resize();
    }

}
