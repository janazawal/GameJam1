using UnityEngine;
//Put sound you want in enum then in its script write "SoundManager.PlaySound(SoundType.X)"
public enum SoundType
{
    Button,
    Win,
    Lose,
    Timer,
    Walk,
    PickUp,
    HitEnemy,
    Konckout,


}
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("Clips")]
    [SerializeField] private AudioClip[] soundList;
    [Header("Music Playlist")]
    [SerializeField] private AudioClip[] musicList;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource timerSource; // NEW: dedicated source so the Timer sound can be stopped independently of other SFX
    private int currentMusicIndex;
    private bool playlistActive;
    private bool musicStopped = false;
    private bool trackStarted = false;

    //extra function linked to volume slider
    private void LoadVolumes()
    {
        musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume", 1f);
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadVolumes();
    }
    private void Start()
    {
        StartPlaylist();
    }
    private void Update()
    {
        HandlePlaylist();
    }
    /*  PLAYLIST  */
    private void StartPlaylist()
    {
        if (musicList.Length == 0)
            return;
        playlistActive = true;
        currentMusicIndex = Mathf.Clamp(currentMusicIndex, 0, musicList.Length - 1);
        PlayCurrentTrack();
    }
    private void PlayCurrentTrack()
    {
        musicSource.clip = musicList[currentMusicIndex];
        musicSource.loop = false;
        musicSource.Play();
    }
    private void HandlePlaylist()
    {
        if (!playlistActive || musicStopped)
            return;
        if (musicSource.isPlaying)
        {
            trackStarted = true;
            return;
        }
        if (trackStarted)
        {
            trackStarted = false;
            PlayNextTrack();
        }
    }
    private void PlayNextTrack()
    {
        currentMusicIndex = (currentMusicIndex + 1) % musicList.Length;
        PlayCurrentTrack();
    }
    /*  SFX  */
    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        if (Instance == null) return;

        // NEW: route Timer through its own dedicated source (looping, stoppable)
        if (sound == SoundType.Timer)
        {
            Instance.timerSource.clip = Instance.soundList[(int)sound];
            Instance.timerSource.loop = true;
            Instance.timerSource.volume = volume;
            Instance.timerSource.Play();
            return;
        }

        Instance.sfxSource.PlayOneShot(
            Instance.soundList[(int)sound],
            volume
        );
    }

    // NEW: stops the Timer sound specifically without affecting other SFX
    public static void StopSound(SoundType sound)
    {
        if (Instance == null) return;

        if (sound == SoundType.Timer)
        {
            Instance.timerSource.Stop();
        }
        // other SoundTypes go through PlayOneShot and can't be individually stopped

    }
    public static void StopSFX()
    {
        if (Instance == null) return;
        Instance.sfxSource.Stop();
    }
    /*  VOLUME  */
    public static void SetMusicVolume(float volume)
    {
        if (Instance == null) return;
        Instance.musicSource.volume = volume;
    }
    public static void SetSFXVolume(float volume)
    {
        if (Instance == null) return;
        Instance.sfxSource.volume = volume;
    }
    public static void StopMusic()
    {
        if (Instance == null) return;
        Instance.musicStopped = true;
        Instance.playlistActive = false;
        Instance.musicSource.Stop();
    }
    public static void ResumeMusic()
    {
        if (Instance == null) return;
        Instance.musicStopped = false;
        Instance.playlistActive = true;
        Instance.PlayCurrentTrack();
    }
    public float InstanceMusicVolume => musicSource.volume;
    public float InstanceSFXVolume => sfxSource.volume;
}