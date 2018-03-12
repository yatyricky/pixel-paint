using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class PixelReader : MonoBehaviour
{
    public Texture2D sourceTex;
    public GameObject TilemapObj;
    public TileBase WhiteTile;

    void Start()
    {
        Color[] pix = sourceTex.GetPixels(0, 0, sourceTex.width, sourceTex.height);
        Tilemap map = TilemapObj.GetComponent<Tilemap>();

        for (int i = 0; i < sourceTex.width; i++)
        {
            for (int j = 0; j < sourceTex.height; j++)
            {
                Color color = pix[i * sourceTex.width + j];
                Vector3Int pos = new Vector3Int(j, i, 0);
                map.SetTileFlags(pos, TileFlags.None);
                map.SetTile(pos, WhiteTile);
                map.SetTileFlags(pos, TileFlags.None);
                map.SetColor(pos, color);
            }
        }
    }

}