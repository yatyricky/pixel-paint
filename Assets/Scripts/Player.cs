using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Camera CameraObj;
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    public Grid GridObj;
    public Tilemap TileMapObj;
    public Tilemap MarkerOverlay;
    public TileBase EmptyTile;
    public GameScene GameData;

    public Color CurrentColor;

    private bool justDragged = false;
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
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Zoom(2);
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

            if (GameData.IsClickable(position))
            {
                TileMapObj.SetTileFlags(position, TileFlags.None);
                TileMapObj.SetTile(position, EmptyTile);
                TileMapObj.SetTileFlags(position, TileFlags.None);
                TileMapObj.SetColor(position, CurrentColor);

                if (CurrentColor.Equals(GameData.Level.Data[position.y * GameData.Level.Width + position.x]))
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

    private void OnMouseDrag()
    {
        CameraObj.transform.Translate((Vector2.zero - MouseHelper.mouseDelta) *  CameraObj.orthographicSize / CameraObj.transform.position.z / CameraObj.transform.position.z);
        if (MouseHelper.mouseDelta.magnitude > 0.5f)
        {
            justDragged = true;
        }
    }

    private void Zoom(float delta)
    {
        // If the camera is orthographic...
        if (CameraObj.orthographic)
        {
            // ... change the orthographic size based on the change in distance between the touches.
            CameraObj.orthographicSize += delta * orthoZoomSpeed;

            // Make sure the orthographic size never drops below zero.
            CameraObj.orthographicSize = Mathf.Max(CameraObj.orthographicSize, Configs.ZOOM_MIN);
        }
        else
        {
            // Otherwise change the field of view based on the change in distance between the touches.
            CameraObj.fieldOfView += delta * perspectiveZoomSpeed;

            // Clamp the field of view to make sure it's between 0 and 180.
            CameraObj.fieldOfView = Mathf.Clamp(CameraObj.fieldOfView, 0.1f, 179.9f);
        }
    }
}