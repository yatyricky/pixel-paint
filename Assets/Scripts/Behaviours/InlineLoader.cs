using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InlineLoader : MonoBehaviour
{
    [Header("Configs")]
    public float Interval = 0.23f;
    public float Brightness = 0.65f;
    public float TrailBrightness = 0.7f;

    [Header("Objects")]
    public GameObject Tips;
    public GameObject Loader;
    public Image[] Dots;

    private float Elapsed;
    private int Frame;
    private Color CurrentColor;
    private bool isRunning;

    private void Awake()
    {
        isRunning = false;
    }

    void Update()
    {
        if (IsRunning())
        {
            Elapsed += Time.deltaTime;
            if (Elapsed >= Interval)
            {
                Elapsed = Elapsed % Interval;
                if (Frame % Dots.Length == 0)
                {
                    System.Random rnd = new System.Random();
                    float r = (float)rnd.NextDouble() * Brightness + 1f - Brightness;
                    float g = (float)rnd.NextDouble() * Brightness + 1f - Brightness;
                    float b = (float)rnd.NextDouble() * Brightness + 1f - Brightness;
                    CurrentColor = new Color(r, g, b);
                }
                for (int i = 0; i < Dots.Length; i ++)
                {
                    int index = Frame % Dots.Length;
                    if (index >= 0)
                    {
                        float scale = TrailBrightness + index * (1f - TrailBrightness) / (Dots.Length - 1);
                        Dots[index].color = new Color(CurrentColor.r * scale, CurrentColor.g * scale, CurrentColor.b * scale);
                    }
                }
                Frame += 1;
            }
        }
    }

    public void InitiateSpin()
    {
        Elapsed = 0f;
        Frame = 0;
        Loader.SetActive(true);
        Tips.SetActive(false);
        isRunning = true;
    }

    public void Halt()
    {
        Loader.SetActive(false);
        Tips.SetActive(true);
        isRunning = false;
    }

    public bool IsRunning()
    {
        return isRunning;
    }
}
