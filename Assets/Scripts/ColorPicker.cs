using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Text Marker;
    public Color SelColor;
    public Image Background;
    public GameObject CheckMark;

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
        p.CurrentColor = SelColor;
        CheckMark.SetActive(true);
        Prev = this;
    }

    internal void Init(Color color, int i)
    {
        Marker.text = i.ToString();
        SelColor = color;
        Background.color = color;
        if (color.grayscale < 0.5f)
        {
            Marker.color = Color.white;
        }
    }
}
