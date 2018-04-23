using UnityEngine;
using System.Collections;

public class FitResolution : MonoBehaviour
{
    public float FixedHeight;
    public float SizeMultiplier;
    public bool IsHallMain;

    private void Awake()
    {
        if (Configs.SHOULD_CHANGE_WIDTH)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Configs.SCREEN_WIDTH * SizeMultiplier, FixedHeight);

            if (IsHallMain)
            {
                rt.anchoredPosition = new Vector2(Configs.SCREEN_WIDTH * -0.5f, 0);
            }
        }
    }

}
