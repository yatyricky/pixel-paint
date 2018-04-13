using UnityEngine;

public class Utils
{
    public static Color TRANSPARENT = new Color(0, 0, 0, 0);

    public static Color ConvertGreyscale(Color color)
    {
        float grey = color.grayscale * (Configs.AUTO_GREY_SCALE_RANGE_E - Configs.AUTO_GREY_SCALE_RANGE_S) + Configs.AUTO_GREY_SCALE_RANGE_S;
        return new Color(grey, grey, grey, color.a);
    }
}
