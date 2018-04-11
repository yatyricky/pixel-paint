using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Text Marker;
    public Color SelColor;
    public Image Background;
    public GameObject CheckMark;

    private int index;
    private static ColorPicker Prev;

    private void Awake()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(ColorPicked);
    }

    public void ColorPicked()
    {
        if (Prev != null)
        {
            Prev.CheckMark.SetActive(false);
        }
        Player p = GameObject.FindGameObjectWithTag("GameController").GetComponent<Player>();
        p.SetBrushColor(SelColor);
        CheckMark.SetActive(true);
        Prev = this;

        // scroll to middle
        transform.parent.gameObject.GetComponent<PaletteTender>().AnimateTo(index);
    }

    internal void Init(Color color, int i)
    {
        Marker.text = i.ToString();
        index = i;
        SelColor = color;
        Background.color = color;
        if (color.grayscale < Configs.FLIP_MARKER)
        {
            Marker.color = Color.white;
        }
    }
}
