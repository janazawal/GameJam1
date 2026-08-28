using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GoalRequirement
{
    public Items item;
    public int requiredAmount;
}

public class GoalManager : MonoBehaviour
{
    [SerializeField] private Pocket pocket;
    [SerializeField] private GoalRequirement[] requirements; //add as many req to win as you like 
    [SerializeField] private GoalRequirement[] extraItems;

    public event Action OnGoalProgressChanged;
    public event Action OnWin;

    private bool hasWon;

    public IReadOnlyList<GoalRequirement> Requirements => requirements;
    public IReadOnlyList<GoalRequirement> ExtraItems => extraItems;

    private void OnEnable()
    {
        pocket.OnInventoryChanged += CheckProgress;
    }

    private void OnDisable()
    {
        pocket.OnInventoryChanged -= CheckProgress;
    }

    private void Start()
    {
        CheckProgress(); // in case UI needs an initial state
    }

    public int GetCollectedAmount(Items item)
    {
        foreach (var slot in pocket.Slots)
        {
            if (slot.item == item)
                return slot.quantity;
        }
        return 0;
    }

    public int GetExtraCollectedAmount()
    {
        return pocket != null ? pocket.ExtraSlot.quantity : 0;
    }

    private void CheckProgress()
    {
        if (hasWon)
        {
            UnlockNewLevel();
        }

        OnGoalProgressChanged?.Invoke();

        foreach (var req in requirements)
        {
            if (GetCollectedAmount(req.item) < req.requiredAmount)
                return; // not all requirements met yet
        }

        hasWon = true;
        OnWin?.Invoke();
    }

    private void UnlockNewLevel()
    {
        if(SceneManager.GetActiveScene().buildIndex>=PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}