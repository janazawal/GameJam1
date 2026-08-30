using TMPro;
using UnityEngine;

public class KnockoutTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private void Awake()
    {
        Hide();
    }

    public void ShowTime(float time)
    {
        if (timerText == null)
            return;

        timerText.gameObject.SetActive(true);
        timerText.text = time.ToString("0.0");
    }

    public void Hide()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }
}