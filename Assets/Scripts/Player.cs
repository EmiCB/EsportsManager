using UnityEngine;
using System;
using System.Collections.Generic;
using Random=UnityEngine.Random;

/// <summary>
/// Player stat types.
/// </summary>
public enum StatType
{
    AIM,
    COMM,
    STRAT,
    REACT
}

public class Player : MonoBehaviour
{
    // hidden stats
    private int level;
    private int statCap;
    private int salary;

    // player visual attributes
    private string name;
    //[SerializeField]
    //private Sprite2D sprite;

    // player stat values
    private int aim;
    private int communication;
    private int strategy;
    private int reaction;

    // configuration constants
    private const int MIN_STAT_VALUE = 1;
    private const int BASE_STAT_BUDGET = 100;
    private const int STAT_BUDGET_PER_LEVEL = 20;
    private const int MAX_STAT_CAP = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: randomize level based on budget (need to implement money system first)
        InitializePlayer(1);
        Debug.Log(name + " " + GetStat(StatType.AIM) + " " + GetStat(StatType.COMM) + " " + GetStat(StatType.STRAT) + " " + GetStat(StatType.REACT));
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Initialize player with given level and randomized stats.
    /// </summary>
    private void InitializePlayer(int playerLevel)
    {
        level = playerLevel;
        statCap = Mathf.Min(50 + (level * 10), MAX_STAT_CAP);

        GenerateRandomName();
        GenerateRandomizedStats();
        CalculateSalary();
    }

    /// <summary>
    /// generate a random name for this player.
    /// </summary>
    private void GenerateRandomName()
    {
        // TODO: update these filler ones ?
        string[] firstNames = { "Alex", "Jordan", "Casey", "Riley", "Morgan", "Taylor", "Skyler", "Quinn" };
        string[] lastNames = { "Smith", "Chen", "Rodriguez", "Kim", "Williams", "Johnson", "Garcia", "Lee" };

        name = firstNames[Random.Range(0, firstNames.Length)] + " " +
                     lastNames[Random.Range(0, lastNames.Length)];
    }

    /// <summary>
    /// Use weighted random distribution for more balanced characters.
    /// Generates 4 random weights and distributes budget proportionally.
    /// </summary>
    private void GenerateRandomizedStats()
    {
        int totalBudget = BASE_STAT_BUDGET + (level * STAT_BUDGET_PER_LEVEL);

        // Generate random weights for each stat
        float aimWeight = Random.Range(0.5f, 1.5f);
        float commWeight = Random.Range(0.5f, 1.5f);
        float stratWeight = Random.Range(0.5f, 1.5f);
        float reactWeight = Random.Range(0.5f, 1.5f);

        float totalWeight = aimWeight + commWeight + stratWeight + reactWeight;

        // Distribute budget based on weights, ensuring minimums
        aim = Mathf.Max(MIN_STAT_VALUE, Mathf.RoundToInt((totalBudget * aimWeight / totalWeight)));
        communication = Mathf.Max(MIN_STAT_VALUE, Mathf.RoundToInt((totalBudget * commWeight / totalWeight)));
        strategy = Mathf.Max(MIN_STAT_VALUE, Mathf.RoundToInt((totalBudget * stratWeight / totalWeight)));
        reaction = Mathf.Max(MIN_STAT_VALUE, Mathf.RoundToInt((totalBudget * reactWeight / totalWeight)));

        // Handle rounding errors - distribute any remaining points
        int currentTotal = aim + communication + strategy + reaction;
        int difference = totalBudget - currentTotal;

        // pick a random stat to add difference to 
        StatType randomStat = (StatType)Random.Range(0, 3);
        if (difference != 0) UpdateStat(randomStat, difference);

        // Clamp all stats to valid range
        aim = Mathf.Clamp(aim, MIN_STAT_VALUE, statCap);
        communication = Mathf.Clamp(communication, MIN_STAT_VALUE, statCap);
        strategy = Mathf.Clamp(strategy, MIN_STAT_VALUE, statCap);
        reaction = Mathf.Clamp(reaction, MIN_STAT_VALUE, statCap);
    }

    private void CalculateSalary()
    {
        return;
    }

    /// <summary>
    /// Get the value of the desired stat.
    /// </summary>
    /// <param name="statType">the stat to retrieve.</param>
    public int GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.AIM:
                return aim;
            case StatType.COMM:
                return communication;
            case StatType.STRAT:
                return strategy;
            case StatType.REACT:
                return reaction;
            default:
                Debug.LogError("No statType was provided to get!");
                return -1;
        }
    }

    /// <summary>
    /// Update the given stat by a value (can be positive or negative). Capped at impossible values.
    /// </summary>
    /// <param name="statType">the stat to modify.</param>
    /// <param name="amount">the amoutn to add/subrtact from the stat.</param>
    public void UpdateStat(StatType statType, int amount)
    {
        switch (statType)
        {
            case StatType.AIM:
                aim = AddRawStat(aim, amount);
                break;
            case StatType.COMM:
                communication = AddRawStat(communication, amount);
                break;
            case StatType.STRAT:
                strategy = AddRawStat(strategy, amount);
                break;
            case StatType.REACT:
                reaction = AddRawStat(reaction, amount);
                break;
            default:
                Debug.LogError("No statType was provided to update!");
                break;
        }
    }

    /// <summary>
    /// Update the raw stat value by an amount (can be positive or negative). Capped at impossible values.
    /// </summary>
    /// <param name="stat">the raw stat value.</param>
    /// <param name="amount">the amoutn to add/subrtact from the stat.</param>
    private int AddRawStat(int stat, int amount)
    {
        if (stat + amount < MIN_STAT_VALUE) return MIN_STAT_VALUE;
        else if (stat + amount > statCap) return statCap;
        else return stat + amount;
    }
}
