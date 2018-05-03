using UnityEngine;
using UnityEngine.UI;

public class BetterToggle : MonoBehaviour
{
    public bool IsOn;
    public Image Icon;
    public Material Greyscale;

    private BetterToggleGroup Group;

    private void Awake()
    {
        Icon.material = new Material(Greyscale);
        if (IsOn == false)
        {
            SetDisable();
        }
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClicked);
        Group = transform.parent.gameObject.GetComponent<BetterToggleGroup>();
    }

    public void OnClicked()
    {
        Group.Activate(this);
    }

    public void SetDisable()
    {
        Icon.material.SetFloat("_EffectAmount", 0.8f);
    }

    public void SetEnable()
    {
        Icon.material.SetFloat("_EffectAmount", 0f);
    }

}
