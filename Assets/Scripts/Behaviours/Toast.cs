using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    public Text MessageObject;

    public float StayTime;
    public float FadeTime;

    private float fadeTimer;

    public void Show(string message)
    {
        MessageObject.text = message;
        gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(0.33f, 0.33f, 0.33f, 1f);
        MessageObject.color = new Color(1f, 1f, 1f, 1f);
        fadeTimer = StayTime + FadeTime;
    }

    private void Update()
    {
        fadeTimer -= Time.deltaTime;
        if (fadeTimer < 0f)
        {
            ClickToDismiss();
        }
        else if (fadeTimer < FadeTime)
        {
            float scale = fadeTimer / FadeTime;
            GetComponent<Image>().color = new Color(0.33f, 0.33f, 0.33f, scale);
            MessageObject.color = new Color(1f, 1f, 1f, scale);
        }
    }

    public void ClickToDismiss()
    {
        gameObject.SetActive(false);
    }

}
