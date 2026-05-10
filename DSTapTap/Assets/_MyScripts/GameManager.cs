using UnityEngine;
using TMPro;
using System.Collections; // Potrebné pre IEnumerator

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Štatistiky")]
    public int totalClicks = 0;
    public int monstersKilled = 0;
    public int coins = 0;
    public int totalCoinsEarned = 0;
    public int totalCoinsSpent = 0;
    public float totalDamageDone = 0f;

    [Header("UI Texty")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI statsClickText;
    public TextMeshProUGUI statsKilledText;
    public TextMeshProUGUI statsEarnedText;
    public TextMeshProUGUI statsSpentText;
    public TextMeshProUGUI statsDamageText;

    [Header("Panely")]
    public GameObject statsPanel;

    [Header("Crit Upgrade Nastavenia")]
    public float critChance = 5f;        // Šanca v percentách
    public float critMultiplier = 2f;    // Násobiteľ kritického zásahu
    public int critUpgradePrice = 150;
    public TextMeshProUGUI critUpgradePriceText;

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

    public void AddDamageStat(float amount)
    {
        totalDamageDone += amount;
        UpdateUI();
    }

    public void AddKill(int baseReward)
    {
        monstersKilled++;
        int finalReward = Mathf.RoundToInt(baseReward * coinMultiplier);
        coins += finalReward;
        totalCoinsEarned += finalReward;
        UpdateUI();
    }

    // --- UPGRADY ---

    public void UpgradeDamage()
    {
        if (coins >= upgradePrice)
        {
            totalCoinsSpent += upgradePrice;
            coins -= upgradePrice;
            clickDamage *= 1.5f;
            upgradePrice *= 2;
            UpdateUI();
        }
    }

    public void UpgradeCoins()
    {
        if (coins >= coinUpgradePrice)
        {
            totalCoinsSpent += coinUpgradePrice;
            coins -= coinUpgradePrice;
            coinMultiplier *= 1.5f;
            coinUpgradePrice = Mathf.RoundToInt(coinUpgradePrice * 2.25f);
            UpdateUI();
        }
    }

    public void UpgradeCritChance()
    {
        if (coins >= critUpgradePrice)
        {
            totalCoinsSpent += critUpgradePrice;
            coins -= critUpgradePrice;

            critChance += 2f;
            critUpgradePrice = Mathf.RoundToInt(critUpgradePrice * 2.5f);

            UpdateUI();
        }
    }

    // --- RESPANN LOGIKA (Fixnutý Bug) ---

    public void RequestRespawn(GameObject entity, float delay)
    {
        StartCoroutine(HandleRespawn(entity, delay));
    }

    private IEnumerator HandleRespawn(GameObject entity, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (entity != null)
        {
            // Skúsime nájsť skript MedusaRespawn (ten testovací)
            var respawnScript = entity.GetComponent<MedusaRespawn>();
            if (respawnScript != null)
            {
                respawnScript.ResetMedusa();
            }
            else
            {
                // Ak používaš hlavný EnemyHealth, zavoláme ten
                var healthScript = entity.GetComponent<EnemyHealth>();
                if (healthScript != null) healthScript.ResetEnemy();
            }
        }
    }

    // --- UI ---

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
        if (coinText != null) coinText.text = "Coins: " + coins;
        if (statsClickText != null) statsClickText.text = "Total Clicks: " + totalClicks;
        if (statsKilledText != null) statsKilledText.text = "Monsters Killed: " + monstersKilled;
        if (statsEarnedText != null) statsEarnedText.text = "Total Earned: " + totalCoinsEarned;
        if (statsSpentText != null) statsSpentText.text = "Total Spent: " + totalCoinsSpent;
        if (statsDamageText != null) statsDamageText.text = "Total Damage: " + Mathf.FloorToInt(totalDamageDone);

        if (upgradePriceText != null) upgradePriceText.text = "Price: " + upgradePrice;
        if (coinUpgradePriceText != null) coinUpgradePriceText.text = "Price: " + coinUpgradePrice;
        if (critUpgradePriceText != null) critUpgradePriceText.text = "Price: " + critUpgradePrice;
    }
}