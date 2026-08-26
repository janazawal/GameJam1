
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseSystem : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    bool isPaused;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(isPaused)
            { Resume(); }
            else { Pause(); }
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Paused");
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        Debug.Log("Resumed");
    }
    public void Home(int SceneID)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneID);
    }
}

