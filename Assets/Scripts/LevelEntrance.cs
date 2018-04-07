using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEntrance : MonoBehaviour
{
    public SpriteRenderer Art;

    private LevelAsset mData;
    private LevelAsset mSave;
    private string mName;

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
            GameScene.DispatchSetGameData(mData, mName, mSave);
            SceneManager.LoadScene("Game");
        }
    }

    internal void SetData(LevelAsset data, string name, LevelAsset save)
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
        if (save != null)
        {
            image.SetPixels(save.Data);
        }
        else
        {
            Color[] greyed = new Color[data.Data.Length];
            for (int i = 0; i < data.Data.Length; i ++)
            {
                greyed[i] = Utils.ConvertGreyscale(data.Data[i]);
            }
            image.SetPixels(greyed);
        }
        image.filterMode = FilterMode.Point;
        image.Apply();
        Art.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        mData = data;

        mSave = save;
        mName = name;
    }
}
