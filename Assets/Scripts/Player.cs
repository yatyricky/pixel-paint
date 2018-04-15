using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera CameraObj;

    public Grid GridObj;
    public GameScene GameManager;

    [HideInInspector] public Color CurrentColor;

    private bool justDragged = false;
    private bool fillMode = false;
    private float fillModeTimer = 0f;
    private Vector2 fillModeMovement = Vector2.zero;
    private float maxCam;
    private bool hasMoreFingerTouched;

    void Update()
    {
        if (Input.touchCount > 1)
        {
            hasMoreFingerTouched = true;
        }
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
            Zoom(deltaMagnitudeDiff / Screen.width * CameraObj.orthographicSize);
            UpdateMarkers();
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

    internal void SetBrushColor(Color selColor)
    {
        CurrentColor = selColor;
        GameManager.HighlightInCanvas(selColor);
    }

    private void OnMouseDown()
    {
        hasMoreFingerTouched = false;
        justDragged = false;
        fillModeTimer = 0f;
        fillModeMovement = Vector2.zero;
        fillMode = false;
    }

    private void FillCell()
    {
        Ray ray = CameraObj.ScreenPointToRay(Input.mousePosition);
        Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        Vector3Int position = GridObj.WorldToCell(worldPoint);
        GameManager.Fill(position, CurrentColor);
    }

    // clicked on a pixel
    private void OnMouseUp()
    {
        if (justDragged == false && !hasMoreFingerTouched)
        {
            FillCell();
            // Touched, should save
            GameManager.touched = true;
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
        // if (true)
        if (Input.touchCount == 1)
        {
            if (fillMode)
            {
                FillCell();
            }
            else
            {
                float x = 0f - CameraObj.orthographicSize * 2 * MouseHelper.mouseDelta.x / Screen.width / Configs.WINDOW_RATIO;
                float y = 0f - CameraObj.orthographicSize * 2 * MouseHelper.mouseDelta.y / Screen.height / Configs.WINDOW_HEIGHT * Configs.DESIGN_HEIGHT;
                CameraObj.transform.Translate(new Vector3(x, y, 0));
                if (MouseHelper.mouseDelta.magnitude > 0.5f)
                {
                    justDragged = true;
                }

                // Detecting fill mode
                fillModeTimer += Time.deltaTime;
                fillModeMovement += MouseHelper.mouseDelta;
                if (fillModeTimer > Configs.HOLD_TO_FILL_TIME)
                {
                    if (fillModeMovement.magnitude < Configs.HOLD_TO_FILL_DISTANCE)
                    {
                        fillMode = true;
                    }
                }
            }
        }
    }

    public void UpdateMarkers()
    {
        if (CameraObj.orthographic)
        {
            GameManager.UpdateMarkers();
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