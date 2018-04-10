using UnityEngine;

public class Utils
{
    public static Color TRANSPARENT = new Color(0, 0, 0, 0);

    public static Color ConvertGreyscale(Color color)
    {
        float grey = color.grayscale * 0.7f + 0.15f;
        return new Color(grey, grey, grey, color.a);
    }
}
