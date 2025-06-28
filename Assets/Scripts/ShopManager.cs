using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public DicePool dicePool;
    public Transform shopUIContainer;
    public GameObject shopItemUIPrefab;

    public int rerollCost = 2;

    private List<ShopItem> currentOffers = new List<ShopItem>();

    void Start()
    {
        GenerateShopOffers();
        DisplayShop();
        HideShop();
    }

    public void GenerateShopOffers()
    {
        currentOffers.Clear();

        for (int i = 0; i < 3; i++)
        {
            float rand = UnityEngine.Random.value;
            if (rand < 0.5f)
            {
                // 50% chance = Dice
                DiceData dice = dicePool.GetRandomDice();
                currentOffers.Add(new ShopItem(dice));
            }
            else if (rand < 0.75f)
            {
                // 25% = Stat Boost
                currentOffers.Add(new ShopItem(ShopItemType.StatBoost, "Gain +20% attack speed next battle", 3));
            }
            else
            {
                // 25% = Skill
                currentOffers.Add(new ShopItem(ShopItemType.Skill, "New Skill: Fire Trail (burn enemies)", 4));
            }
        }
    }

    public void DisplayShop()
    {
        foreach (Transform child in shopUIContainer)
            Destroy(child.gameObject);

        foreach (var item in currentOffers)
        {
            GameObject go = Instantiate(shopItemUIPrefab, shopUIContainer);
            ShopItemUI ui = go.GetComponent<ShopItemUI>();
            ui.Setup(item, this);
        }
    }

    public void AttemptPurchase(ShopItem item)
{
    if (PlayerCurrency.Instance.SpendGold(item.cost))
    {
        Debug.Log($"Purchased: {item.description}");

        if (item.itemType == ShopItemType.Dice)
        {
            // Add to player's dice collection
        }

        currentOffers.Remove(item); // ✅ Remove only the one bought
        DisplayShop();             // ✅ Re-render remaining items
    }
    else
    {
        Debug.Log("Not enough gold!");
    }
}

void OnEnable()
{
    GameEvents.OnCombatStarted += HideShop;
    GameEvents.OnCombatEnded += ShowShop;
}

void OnDisable()
{
    GameEvents.OnCombatStarted -= HideShop;
    GameEvents.OnCombatEnded -= ShowShop;
}


void OpenShop()
{
    shopUIContainer.gameObject.SetActive(true);
    GameEvents.RaiseShopOpened();
    GenerateShopOffers();
    DisplayShop();
}




    public void Reroll()
    {
        if (PlayerCurrency.Instance.SpendGold(rerollCost))
        {
            GenerateShopOffers();
            DisplayShop();
        }
        else
        {
            Debug.Log("Not enough gold to reroll!");
        }
    }

    public void ShowShop()
    {
        shopUIContainer.gameObject.SetActive(true);
        GenerateShopOffers();
        DisplayShop();
    }

    public void HideShop()
    {
        shopUIContainer.gameObject.SetActive(false);
    }
}
