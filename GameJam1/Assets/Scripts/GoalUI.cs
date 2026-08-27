using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalUI : MonoBehaviour
{
    [SerializeField] private GoalManager goalManager;
    [SerializeField] private Image[] goalImages;
    [SerializeField] private TMP_Text[] goalCountTexts; // shows "current/required"
    [SerializeField] private GameObject winPanel; // optional, shown on win

    private void OnEnable()
    {
        goalManager.OnGoalProgressChanged += RefreshUI;
        goalManager.OnWin += ShowWin;
        RefreshUI();
    }

    private void OnDisable()
    {
        goalManager.OnGoalProgressChanged -= RefreshUI;
        goalManager.OnWin -= ShowWin;
    }

    private void RefreshUI()
    {
        var requirements = goalManager.Requirements;

        for (int i = 0; i < goalImages.Length && i < requirements.Count; i++)
        {
            var req = requirements[i];
            int collected = goalManager.GetCollectedAmount(req.item);

            goalImages[i].sprite = req.item.sprite;
            goalImages[i].enabled = true;

            bool complete = collected >= req.requiredAmount;
            goalImages[i].color = complete ? Color.white : new Color(1, 1, 1, 0.6f);

            if (goalCountTexts[i])
                goalCountTexts[i].text = $"{collected}/{req.requiredAmount}";
        }
    }

    private void ShowWin()
    {
        if (winPanel) winPanel.SetActive(true);
        Debug.Log("Player won!");
    }
}