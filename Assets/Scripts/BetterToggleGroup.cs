using UnityEngine;
using System.Collections;
using System;

public class BetterToggleGroup : MonoBehaviour
{

    internal void Activate(BetterToggle betterToggle)
    {
        foreach (Transform child in transform)
        {
            BetterToggle toggle = child.gameObject.GetComponent<BetterToggle>();
            if (toggle == betterToggle)
            {
                toggle.SetEnable();
            }
            else
            {
                toggle.SetDisable();
            }
        }
    }
}
