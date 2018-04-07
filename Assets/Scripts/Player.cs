using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Camera CameraObj;

    public Grid GridObj;
    public Tilemap TileMapObj;
    public Tilemap MarkerOverlay;
    public TileBase EmptyTile;
    public GameScene GameManager;

    [HideInInspector] public Color CurrentColor;

    private bool justDragged = false;
    private float maxCam;
    public static Color TRANSPARENT = new Color(0, 0, 0, 0);

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            Debug.Log(deltaMagnitudeDiff);

            Zoom(deltaMagnitudeDiff / 10.0f);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            Zoom(-2);
            UpdateMarkers();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Zoom(2);
            UpdateMarkers();
        }
    }

    private void OnMouseDown()
    {
        justDragged = false;
    }

    // clicked on a pixel
    private void OnMouseUp()
    {
        if (justDragged == false)
        {
            Ray ray = CameraObj.ScreenPointToRay(Input.mousePosition);
            // get the collision point of the ray with the z = 0 plane
            Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            Vector3Int position = GridObj.WorldToCell(worldPoint);

            if (GameManager.IsClickable(position))
            {
                TileMapObj.SetTileFlags(position, TileFlags.None);
                TileMapObj.SetTile(position, EmptyTile);
                TileMapObj.SetTileFlags(position, TileFlags.None);
                TileMapObj.SetColor(position, CurrentColor);

                if (CurrentColor.Equals(GameManager.Level.Data[position.y * GameManager.Level.Width + position.x]))
                {
                    MarkerOverlay.SetTileFlags(position, TileFlags.None);
                    MarkerOverlay.SetColor(position, TRANSPARENT);
                }
                else
                {
                    MarkerOverlay.SetTileFlags(position, TileFlags.None);
                    MarkerOverlay.SetColor(position, Color.white);
                }
            }
        }
    }

    internal void ZoomTo(float camSize)
    {
        maxCam = camSize * 2;
        if (camSize < Configs.ZOOM_MIN)
        {
            camSize = Configs.ZOOM_MIN;
        }
        CameraObj.orthographicSize = camSize;
        UpdateMarkers();
    }

    private void OnMouseDrag()
    {
        CameraObj.transform.Translate((Vector2.zero - MouseHelper.mouseDelta) *  CameraObj.orthographicSize / CameraObj.transform.position.z / CameraObj.transform.position.z);
        if (MouseHelper.mouseDelta.magnitude > 0.5f)
        {
            justDragged = true;
        }
    }

    public void UpdateMarkers()
    {
        if (CameraObj.orthographic)
        {
            GameManager.UpdateMarkers(CameraObj.orthographicSize);
        }
        // else not implemented
    }

    private void Zoom(float delta)
    {
        if (CameraObj.orthographic)
        {
            float targetSize = CameraObj.orthographicSize + delta;
            targetSize = Mathf.Min(targetSize , maxCam);
            targetSize = Mathf.Max(targetSize, Configs.ZOOM_MIN);
            CameraObj.gameObject.GetComponent<LerpZoom>().TargetSize = targetSize;
        }
    }
}