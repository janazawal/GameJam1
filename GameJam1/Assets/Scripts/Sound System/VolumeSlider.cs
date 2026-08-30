using UnityEngine;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType { Music, SFX }
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private VolumeType volumeType;
    private string prefKey;
    private void Awake()
    {
        prefKey = volumeType == VolumeType.Music ? "musicVolume" : "sfxVolume";
    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey(prefKey))
        {
            PlayerPrefs.SetFloat(prefKey, 1f);
        }
        Load();
    }
    private void OnEnable()
    {
        SyncSliderToCurrentVolume();
    }
    private void SyncSliderToCurrentVolume()
    {
        if (volumeType == VolumeType.Music)
        {
            if (MenuMusicController.Instance == null) return;
            volumeSlider.SetValueWithoutNotify(MenuMusicController.Instance.Volume);
        }
        else
        {
            if (SoundManager.Instance == null) return;
            volumeSlider.SetValueWithoutNotify(SoundManager.Instance.InstanceSFXVolume);
        }
    }
    public void ChangeVolume()
    {
        ApplyVolume(volumeSlider.value);
        Save();
    }
    private void Save()
    {
        PlayerPrefs.SetFloat(prefKey, volumeSlider.value);
    }
    private void Load()
    {
        float saved = PlayerPrefs.GetFloat(prefKey);
        volumeSlider.value = saved;
        ApplyVolume(saved);
    }
    private void ApplyVolume(float value)
    {
        if (volumeType == VolumeType.Music)
        {
            if (MenuMusicController.Instance != null)
                MenuMusicController.Instance.Volume = value;
        }
        else
        {
            SoundManager.SetSFXVolume(value);
        }
    }
}