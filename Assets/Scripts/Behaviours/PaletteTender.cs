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
        int target = index - 3;
        if (target > size - 5)
        {
            target = size - 5;
        }
        if (target < 0)
        {
            target = 0;
        }
        TargetPos = -1 * target * Configs.PALETTE_WIDTH;
    }

}
