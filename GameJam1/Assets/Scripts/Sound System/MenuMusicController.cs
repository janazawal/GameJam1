using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(AudioSource))]
public class MenuMusicController : MonoBehaviour
{
    [SerializeField] private string[] allowedScenes = { "MainMenu" };
    private AudioSource audioSource;
    private static MenuMusicController instance;
    public static MenuMusicController Instance => instance;
    public float Volume
    {
        get => audioSource != null ? audioSource.volume : 1f;
        set { if (audioSource != null) audioSource.volume = value; }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }
    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool shouldPlay = System.Array.IndexOf(allowedScenes, scene.name) >= 0;
        if (shouldPlay)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}