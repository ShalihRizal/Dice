using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsCombatActive { get; private set; } = false;

    public int currentDay { get; private set; } = 1;
    public int currentHour { get; private set; } = 1;
    private const int maxHoursPerDay = 10;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartCombat()
{
    IsCombatActive = true;
    GameEvents.RaiseCombatStarted();
    FindObjectOfType<EnemySpawner>()?.StartCombat();
}

public void EndCombat()
{
    GiveCombatRewards();
    IsCombatActive = false;
    GameEvents.RaiseCombatEnded();
    Debug.Log("Combat finished.");
    AdvanceTime();
}



    void AdvanceTime()
    {
        currentHour++;
    if (currentHour > maxHoursPerDay)
{
    currentHour = 1;
    currentDay++;
}


    GameEvents.OnTimeChanged?.Invoke(currentDay, currentHour);
    }

    void GiveCombatRewards()
    {
        int reward = UnityEngine.Random.Range(5, 11);
        PlayerCurrency.Instance.AddGold(reward); // Adjusted to match latest
        Debug.Log($"💰 Earned {reward} coins!");
    }
}
