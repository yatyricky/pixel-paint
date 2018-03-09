using UnityEngine;
using System.Collections;

public class PixelReader : MonoBehaviour
{
    public Texture2D sourceTex;
    public GameObject PixelsContainerObj;
    public GameObject TilePrefab;

    void Start()
    {
        Color[] pix = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
        //GetComponent<SpriteRenderer>().sprite = Sprite.Create

        for (int i = 0; i < sourceTex.width; i ++)
        {
            for (int j = 0; j < sourceTex.height; j ++)
            {
                GameObject tile = Instantiate(TilePrefab);
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                Color color = pix[i * sourceTex.width + j];
                Texture2D destTex = new Texture2D(1, 1);
                destTex.SetPixels(new Color[] { color });
                destTex.Apply();
                sr.sprite = Sprite.Create(destTex, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0.5f, 0.5f), 1);
                tile.transform.position = new Vector3(j, i, 0f);
                tile.transform.parent = PixelsContainerObj.transform;
            }
        }
    }
}