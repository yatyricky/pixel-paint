using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEntrance : MonoBehaviour
{
    public SpriteRenderer Art;

    private LevelAsset mData;

    private void Awake()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClicked()
    {
        if (mData == null)
        {
            // TODO TOAST
        }
        else
        {
            // TODO SPINNER
            GameScene.DispatchSetGameData(mData);
            SceneManager.LoadScene("Game");
        }
    }

    internal void SetData(LevelAsset data)
    {
        Texture2D image = new Texture2D(data.Width, data.Height);
        float pixelsPerUnit = 0.2f;
        if (data.Height > data.Width)
        {
            pixelsPerUnit = data.Height / Configs.LEVEL_ENTRANCE_FRAME_SIZE;
        }
        else
        {
            pixelsPerUnit = data.Width / Configs.LEVEL_ENTRANCE_FRAME_SIZE;
        }
        image.SetPixels(data.Data);
        image.filterMode = FilterMode.Point;
        image.Apply();
        Art.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        mData = data;
    }
}
