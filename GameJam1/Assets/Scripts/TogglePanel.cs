using UnityEngine;
using System;
public class TogglePanel : MonoBehaviour
{
    [SerializeField]  GameObject panel;
    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);

    }
}
