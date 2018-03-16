using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public Text Marker;
    public Color SelColor;
    public Image Background;

    private void Awake()
    {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(ColorPicked);
    }

    public void ColorPicked()
    {
        Debug.Log("Clicked color is " + SelColor.ToString());
        Player p = GameObject.FindGameObjectWithTag("GameController").GetComponent<Player>();
        p.CurrentColor = SelColor;
    }

    internal void Init(Color color, int i)
    {
        Marker.text = i.ToString();
        SelColor = color;
        Background.color = color;
    }
}
