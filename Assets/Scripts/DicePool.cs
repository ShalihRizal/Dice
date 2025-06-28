using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DicePool", menuName = "Dice/Dice Pool")]
public class DicePool : ScriptableObject
{
    public List<DiceData> allDice;

    public DiceData GetRandomDice()
    {
        float roll = Random.value;

        DiceRarity chosenRarity;
        if (roll < 0.03f) chosenRarity = DiceRarity.Legendary;
        else if (roll < 0.15f) chosenRarity = DiceRarity.Epic;
        else if (roll < 0.40f) chosenRarity = DiceRarity.Rare;
        else chosenRarity = DiceRarity.Common;

        var matchingDice = allDice.Where(d => d.rarity == chosenRarity).ToList();

        if (matchingDice.Count == 0)
            matchingDice = allDice.Where(d => d.rarity == DiceRarity.Common).ToList(); // fallback

        return matchingDice[Random.Range(0, matchingDice.Count)];
    }
}
