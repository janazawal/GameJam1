using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private GoalManager goalManager;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private float startTime = 60f;
    [SerializeField] private bool startOnEnable = true;

    public event Action OnTimerFinished;

    private float timeRemaining;
    private bool isRunning;

    private void OnEnable()
    {
        if (startOnEnable)
            StartTimer(startTime);
    }

    public void StartTimer(float duration)
    {
        timeRemaining = duration;
        isRunning = true;
        UpdateTimerText();
    }

    public void PauseTimer() => isRunning = false;
    public void ResumeTimer() => isRunning = true;

    private void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            UpdateTimerText();
            FinishTimer();
            return;
        }

        UpdateTimerText();
    }

    private void FinishTimer()
    {
        OnTimerFinished?.Invoke();

        if (HasMetAllGoals())
        {
            if (winPanel) winPanel.SetActive(true);
        }
        else
        {
            if (losePanel) losePanel.SetActive(true);
        }
    }

    private bool HasMetAllGoals()
    {
        if (goalManager == null) return false;

        foreach (var req in goalManager.Requirements)
        {
            if (goalManager.GetCollectedAmount(req.item) < req.requiredAmount)
                return false;
        }
        return true;
    }

    private void UpdateTimerText()
    {
        if (!timerText) return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}