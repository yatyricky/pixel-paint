using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlaySoundToggle : MonoBehaviour
{
    private Toggle m_Toggle;

    void Start()
    {
        m_Toggle = GetComponent<Toggle>();
        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(m_Toggle);
        });
        if (DataManager.Instance.AudioManager.IsMute())
        {
            m_Toggle.isOn = true;
        }
    }

    public void ToggleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            DataManager.Instance.AudioManager.Mute(true);
        }
        else
        {
            DataManager.Instance.AudioManager.Mute(false);
        }
    }

}
