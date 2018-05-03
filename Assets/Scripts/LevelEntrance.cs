using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class LevelEntrance : MonoBehaviour
{
    public SpriteRenderer Art;
    public Text Loves;
    public GameObject Liked;
    public GameObject Disliked;

    private LevelAsset mData;
    private LevelAsset mSave;
    public bool beloved;

    private void Start()
    {
        SetLoveNumber(0);
        UpdateLoveUI();
    }

    private void SetLoveNumber(int v)
    {
        if (v <= 0)
        {
            Loves.text = "-";
        }
        else
        {
            Loves.text = v.ToString();
        }
    }

    private void UpdateLoveUI()
    {
        if (beloved == true)
        {
            Disliked.SetActive(false);
            Liked.SetActive(true);
        }
        else
        {
            Liked.SetActive(false);
            Disliked.SetActive(true);
        }
    }

    private void OnClicked()
    {
        if (mData == null)
        {
            // TODO TOAST
        }
        else
        {
            // TODO SPINNER
            GameScene.DispatchSetGameData(mData, mSave);
            SceneManager.LoadScene("Game");
        }
    }

    private void OnClickedLove()
    {
        // update love icon
        beloved = !beloved;
        UpdateLoveUI();
        // update text
        int before;
        if (!Int32.TryParse(Loves.text, out before))
        {
            before = 0;
        }
        if (beloved)
        {
            SetLoveNumber(before + 1);
        }
        else
        {
            SetLoveNumber(before - 1);
        }
        // update local file
        DataManager.Instance.CommitLove(mData.Name, beloved);
    }

    internal void SetData(LevelAsset data, LevelAsset save)
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
                greyed[i] = SmallTricks.Utils.ConvertGreyscale(data.Data[i]);
            }
            image.SetPixels(greyed);
        }
        image.filterMode = FilterMode.Point;
        image.Apply();
        Art.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        mData = data;

        mSave = save;
        StartCoroutine(UpdateLoves());
    }

    private IEnumerator UpdateLoves()
    {
        string request = Configs.SERVER + ":" + Configs.PORT + "/getdata?name=" + mData.Name + "&secret=" + Configs.SECRET;
        WWW www = new WWW(request);
        yield return www;
        PixelData data = JsonUtility.FromJson<PixelData>(www.text);
        SetLoveNumber(data.loves);
    }

    private class PixelData
    {
        public int loves;
        public string[] tags;
    }

    public class LoveData
    {
        public string[] data;
    }
}
