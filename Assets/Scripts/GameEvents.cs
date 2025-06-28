using System;
using UnityEngine;

public static class GameEvents
{
    // Combat
    public static event Action OnCombatStarted;
    public static event Action OnCombatEnded;

    // Shop
    public static event Action OnShopOpened;
    public static event Action OnShopClosed;

    // Currency
    public static event Action<int> OnCurrencyChanged;

    // Cycle
    public static Action<int, int> OnTimeChanged;

    // Methods to call
    public static void RaiseCombatStarted() => OnCombatStarted?.Invoke();
    public static void RaiseCombatEnded() => OnCombatEnded?.Invoke();

    public static void RaiseShopOpened() => OnShopOpened?.Invoke();
    public static void RaiseShopClosed() => OnShopClosed?.Invoke();

    public static void RaiseCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);
}
