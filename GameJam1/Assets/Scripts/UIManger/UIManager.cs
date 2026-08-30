using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GoalManager goalManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    } 

    void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    private void OnEnable()
    {
        goalManager.OnWin += ShowWinPanel;
    }

    private void OnDisable()
    {
        goalManager.OnWin -= ShowWinPanel;
    }
    public void ShowWinPanel()
    {

        SoundManager.StopSound(SoundType.Timer);
        SoundManager.PlaySound(SoundType.Win);
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowLosePanel()
    {
        SoundManager.StopSound(SoundType.Timer);
        SoundManager.PlaySound(SoundType.Lose);
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        SoundManager.StopSFX();
        SoundManager.PlaySound(SoundType.Button);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SoundManager.StopSFX();
        SoundManager.PlaySound(SoundType.Button);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }

    public void LoadNextLevel()
    {
        SoundManager.StopSFX();
        SoundManager.PlaySound(SoundType.Button);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
