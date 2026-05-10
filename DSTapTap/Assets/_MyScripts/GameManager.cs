using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Štatistiky")]
    public int totalClicks = 0;
    public int monstersKilled = 0;
    public int coins = 0;

    [Header("UI Texty")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI statsClickText;
    public TextMeshProUGUI statsKilledText;

    [Header("Panely")]
    public GameObject statsPanel;

    [Header("Damage Upgrade (DMG)")]
    public float clickDamage = 10f;
    public int upgradePrice = 50;
    public TextMeshProUGUI upgradePriceText;

    [Header("Coins Upgrade (Multiplier)")]
    public float coinMultiplier = 1.0f;
    public int coinUpgradePrice = 125;
    public TextMeshProUGUI coinUpgradePriceText;

    void Awake()
    {
        // Singleton pattern - nastavíme inštanciu hneď pri zrodení
        if (instance == null) instance = this;
    }

    void Start()
    {
        UpdateUI();
        if (statsPanel != null) statsPanel.SetActive(false);
    }

    // --- LOGIKA HRY ---

    public void AddClick()
    {
        totalClicks++;
        UpdateUI();
    }

    public void AddKill(int baseReward)
    {
        monstersKilled++;

        // Výpočet odmeny s násobiteľom a zaokrúhlenie na celé číslo
        int finalReward = Mathf.RoundToInt(baseReward * coinMultiplier);
        coins += finalReward;

        UpdateUI();
    }

    // Funkcia pre Damage Upgrade Button
    public void UpgradeDamage()
    {
        if (coins >= upgradePrice)
        {
            coins -= upgradePrice;
            clickDamage *= 1.5f;
            upgradePrice *= 2;
            UpdateUI();
        }
        else
        {
            Debug.Log("Malo peňazí na DMG upgrade!");
        }
    }

    // Funkcia pre Coins Multiplayer Button
    public void UpgradeCoins()
    {
        if (coins >= coinUpgradePrice)
        {
            coins -= coinUpgradePrice;
            coinMultiplier *= 1.5f;
            // Cena rastie o 2.25x a zaokrúhlime ju
            coinUpgradePrice = Mathf.RoundToInt(coinUpgradePrice * 2.25f);
            UpdateUI();
        }
        else
        {
            Debug.Log("Malo peňazí na Coins upgrade!");
        }
    }

    // --- UI FUNKCIE ---

    public void ToggleStats()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(!statsPanel.activeSelf);
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        // Základné UI
        if (coinText != null) coinText.text = "Coins: " + coins;
        if (statsClickText != null) statsClickText.text = "Total Clicks: " + totalClicks;
        if (statsKilledText != null) statsKilledText.text = "Monsters Killed: " + monstersKilled;

        // Upgrade DMG Text
        if (upgradePriceText != null) upgradePriceText.text = "Price: " + upgradePrice;

        // Upgrade Coins Text
        if (coinUpgradePriceText != null) coinUpgradePriceText.text = "Price: " + coinUpgradePrice;
    }
}