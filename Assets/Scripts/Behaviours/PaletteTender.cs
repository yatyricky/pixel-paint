using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PaletteTender : MonoBehaviour
{
    public GameScene GameManager;

    private float TargetPos = 0f;
    private Vector3 velocity = Vector3.zero;
    private bool Animating = false;

    private void Update()
    {
        if (Animating && Mathf.Abs(TargetPos - transform.localPosition.x) > Configs.BIG_EPSILON)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(TargetPos, transform.localPosition.y, 0), ref velocity, 0.1f);
        }
        else
        {
            Animating = false;
        }
    }

    private void OnMouseDown()
    {
        Animating = false;
    }

    public void AnimateTo(int index)
    {
        Animating = true;
        int size = GameManager.Level.Palette.Length;
        float numberPerRow = Configs.SCREEN_WIDTH / Configs.PALETTE_WIDTH;
        float middle = numberPerRow / 2f + 0.5f;
        float target = index - middle;
        if (target > size - numberPerRow)
        {
            target = size - numberPerRow;
        }
        if (target < 0)
        {
            target = 0;
        }
        TargetPos = -1 * target * Configs.PALETTE_WIDTH;
    }

}
