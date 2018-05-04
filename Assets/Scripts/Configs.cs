// Magic number class
public class Configs
{
    // ACTION
    public const float ZOOM_MIN = 7f;
    public const float ZOOM_FADE_MARKER = 12f;
    public const float ZOOM_HIDE_MARKER = 24f;
    public const float ART_IN_WINDOW_RATIO = 0.5f;
    public const float HOLD_TO_FILL_TIME = 0.3f;
    public const float HOLD_TO_FILL_DISTANCE = 1.5f;
    public const float HIGHLIGHT_CANVAS_RATIO = 0.8f;
    public const float AUTO_GREY_SCALE_RANGE_S = 0.5f;
    public const float AUTO_GREY_SCALE_RANGE_E = 0.9f;
    public const float FLIP_MARKER = 0.5f; // is marker white or black
    public const float BIG_EPSILON = 0.01f;
    public const float INTERSTITIAL_AD_CD = 60f;

    // UI
    public const float DESIGN_HEIGHT = 1280f;
    public const float DESIGN_WIDTH = 720f;
    public static float SCREEN_WIDTH = 720f;
    public static bool SHOULD_CHANGE_WIDTH = false;
    public const float STANDARD_RATIO = DESIGN_WIDTH / DESIGN_HEIGHT;
    public const float WINDOW_HEIGHT = 1280f;
    public const float WINDOW_RATIO = WINDOW_HEIGHT / DESIGN_WIDTH;
    public const float PIXEL_WIDTH_CAM_RATIO = 1.42f;
    public const float PALETTE_WIDTH = 144;
    public const float LEVEL_DESIGN_WIDTH_RATIO = 0.47f;
    public static float LEVEL_WIDTH = 339f;
    public static float LEVEL_HEIGHT = 410f;
    public const float LEVEL_ENTRANCE_FRAME_SIZE = 0.375f;
    public static float LEVEL_RATIO = LEVEL_WIDTH / LEVEL_HEIGHT;
    public static float LEVEL_MARGIN = 14f;

    // NETWORK
    public const string SERVER = "https://g.nefti.me";
    public const string PORT = "11367";
    public const string SECRET = "URNwTDLK-iNYIzZ6P-YAV1l5pK-PvKZuFb9";
}
