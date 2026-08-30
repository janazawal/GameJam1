using UnityEngine;

public class Quit : MonoBehaviour
{
    public void QuitGame()
    {
        SoundManager.PlaySound(SoundType.Button);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
