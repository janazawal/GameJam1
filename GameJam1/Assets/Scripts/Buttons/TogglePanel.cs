using UnityEngine;
using System;
public class TogglePanel : MonoBehaviour
{
    [SerializeField]  GameObject panel;
    public void Toggle()
    {
        SoundManager.PlaySound(SoundType.Button);
        panel.SetActive(!panel.activeSelf);

    }
}
