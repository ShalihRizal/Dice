using UnityEngine;

[CreateAssetMenu(fileName = "NewDiceData", menuName = "Dice/Dice Data")]
public class DiceData : ScriptableObject
{
    public string diceName;
    public GameObject prefab;
    public DiceRarity rarity;
    public int sides;
    public float baseDamage;
    public Dice.PassiveAbility passive;
    public int cost;
}
