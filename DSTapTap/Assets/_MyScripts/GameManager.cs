using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance => instance;

    [Header("Zariadenie na Výbavu (Prepojenie)")]
    private EquipmentManager equipmentManager;

    [Header("Aktívne % Modifikátory z Výbavy")]
    public float activeDamageBoost = 0f;
    public float activeCoinsBoost = 0f;
    public float activeBossTimeBonus = 0f;
    public float activeCritChanceBoost = 0f;

    [Header("Štatistiky")]
    public int totalClicks = 0;
    public int monstersKilled = 0;
    public int coins = 0;
    public int totalCoinsEarned = 0;
    public int totalCoinsSpent = 0;
    public float totalDamageDone = 0f;
    public int totalGemsEarned = 0;

    [Header("Herná Mena - Gems")]
    public int gems = 0;
    public TextMeshProUGUI gemsText;
    public TextMeshProUGUI statsGemsText;

    [Header("Typ Nepriateľa")]
    [Tooltip("0 = Medúza, 1 = Ork")]
    public int currentEnemyType = 0;

    [Header("Boss Systém")]
    public int killsToBoss = 10;
    public int currentKills = 0;
    public TextMeshProUGUI bossProgressText;
    public bool nextIsBoss = false;
    public float bossTimeLimit = 15f;

    [Header("UI Texty - Štatistiky")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI statsClickText;
    public TextMeshProUGUI statsKilledText;
    public TextMeshProUGUI statsEarnedText;
    public TextMeshProUGUI statsSpentText;
    public TextMeshProUGUI statsDamageText;

    [Header("Gold Mine Nastavenia")]
    public int mineLevel = 0;
    public int mineBaseReward = 20;
    public float mineInterval = 2f;
    public int mineUpgradePrice = 500;
    public TextMeshProUGUI mineUpgradePriceText;
    public TextMeshProUGUI mineStatusText;

    [Header("Panely")]
    public GameObject statsPanel;

    [Header("Crit Upgrade Nastavenia")]
    public float critChance = 5f;
    public float critMultiplier = 2f;
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
        // Nájdeme EquipmentManager na scéne
        equipmentManager = FindObjectOfType<EquipmentManager>();

        UpdateActiveModifiers();
        UpdateUI();

        if (statsPanel != null) statsPanel.SetActive(false);
        StartCoroutine(GoldMineRoutine());
        UpdateBossUI();
    }

    // Táto funkcia sčíta bonusy LEN z vecí, ktoré sú reálne nasadené v EquipmentSlots
    public void UpdateActiveModifiers()
    {
        // Vynulujeme hodnoty
        activeDamageBoost = 0f;
        activeCoinsBoost = 0f;
        activeBossTimeBonus = 0f;
        activeCritChanceBoost = 0f;

        // VŽDY nájdeme aktuálny EquipmentManager na scéne
        equipmentManager = FindObjectOfType<EquipmentManager>();

        if (equipmentManager != null)
        {
            foreach (var item in equipmentManager.GetEquippedItems().Values)
            {
                if (item != null)
                {
                    activeDamageBoost += item.damageBoostPercent;
                    activeCoinsBoost += item.coinsBoostPercent;
                    activeBossTimeBonus += item.moreBossTimeSeconds;
                    activeCritChanceBoost += item.critChanceBoostPercent;

                    Debug.Log(
                        $"[GameManager] Item bonus pripočítaný: " +
                        $"DMG +{item.damageBoostPercent * 100}% | " +
                        $"Gold +{item.coinsBoostPercent * 100}% | " +
                        $"BossTime +{item.moreBossTimeSeconds}s | " +
                        $"Crit +{item.critChanceBoostPercent * 100}%"
                    );
                }
            }

            Debug.Log(
                $"[GameManager] Modifikátory prepočítané: " +
                $"DMG +{activeDamageBoost * 100}% | " +
                $"Gold +{activeCoinsBoost * 100}% | " +
                $"BossTime +{activeBossTimeBonus}s | " +
                $"Crit +{activeCritChanceBoost * 100}%"
            );
        }
        else
        {
            Debug.LogWarning("[GameManager] EquipmentManager nebol nájdený na scéne!");
        }

        UpdateUI();
    }

    // Vypočíta finálny damage kliku vrátane upgradov, % boostov z výbavy a kritických zásahov
    public float GetFinalClickDamage()
    {
        float calculatedDamage = clickDamage * (1f + activeDamageBoost);

        float totalCritChance = (critChance / 100f) + activeCritChanceBoost;

        if (Random.value < totalCritChance)
        {
            calculatedDamage *= critMultiplier;
            Debug.Log("<color=yellow>🎯 KRITICKÝ ZÁSAH!</color>");
        }

        return calculatedDamage;
    }

    // Vypočíta finálny časový limit na bossa
    public float GetFinalBossTime()
    {
        return bossTimeLimit + activeBossTimeBonus;
    }

    private IEnumerator GoldMineRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(mineInterval);

            if (mineLevel > 0)
            {
                float baseReward = mineBaseReward * Mathf.Pow(2.2f, mineLevel - 1);

                int finalReward = Mathf.RoundToInt(baseReward * coinMultiplier * (1f + activeCoinsBoost));

                coins += finalReward;
                totalCoinsEarned += finalReward;

                UpdateUI();
            }
        }
    }

    public void UpgradeGoldMine()
    {
        if (coins >= mineUpgradePrice)
        {
            totalCoinsSpent += mineUpgradePrice;
            coins -= mineUpgradePrice;
            mineLevel++;
            mineUpgradePrice = Mathf.RoundToInt(mineUpgradePrice * 2.5f);

            UpdateUI();
        }
    }

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

    public void AddGems(int amount)
    {
        gems += amount;
        totalGemsEarned += amount;
        UpdateUI();
    }

    public void AddCoinsFromShop(int amount)
    {
        coins += amount;
        totalCoinsEarned += amount;
        UpdateUI();
    }

    public void AddKill(int baseReward)
    {
        monstersKilled++;

        int finalReward = Mathf.RoundToInt(baseReward * coinMultiplier * (1f + activeCoinsBoost));

        coins += finalReward;
        totalCoinsEarned += finalReward;

        currentEnemyType = Random.Range(0, 2);

        if (nextIsBoss)
        {
            int sanca = Random.Range(1, 101);

            if (sanca <= 5)
            {
                AddGems(1);
                Debug.Log("<color=cyan>Skvelé! Z bossa vypadol vzácny Gem (5% šanca)!</color>");
            }

            currentKills = 0;
            nextIsBoss = false;
        }
        else
        {
            currentKills++;

            if (currentKills >= killsToBoss)
            {
                nextIsBoss = true;
            }
        }

        UpdateBossUI();
        UpdateUI();
    }

    public void BossFailed(GameObject boss)
    {
        currentKills = 0;
        nextIsBoss = false;

        UpdateBossUI();

        if (boss != null)
        {
            var healthScript = boss.GetComponent<EnemyHealth>();

            if (healthScript != null)
            {
                healthScript.ResetEnemy();
            }
        }
    }

    public void UpdateBossUI()
    {
        if (bossProgressText != null)
        {
            if (nextIsBoss)
            {
                bossProgressText.text = "BOSS READY!";
                bossProgressText.color = Color.red;
            }
            else
            {
                bossProgressText.text = currentKills + " / " + killsToBoss;
                bossProgressText.color = Color.white;
            }
        }
    }

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
            critUpgradePrice = Mathf.RoundToInt(critUpgradePrice * 1.3f);

            UpdateUI();
        }
    }

    public void RequestRespawn(GameObject entity, float delay)
    {
        StartCoroutine(HandleRespawn(entity, delay));
    }

    private IEnumerator HandleRespawn(GameObject entity, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (entity != null)
        {
            var healthScript = entity.GetComponent<EnemyHealth>();

            if (healthScript != null)
            {
                healthScript.ResetEnemy();
            }
        }
    }

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
        if (coinText != null) coinText.text = FormatNumbers(coins);
        if (gemsText != null) gemsText.text = FormatNumbers(gems);

        if (statsClickText != null) statsClickText.text = "Total Clicks: " + FormatNumbers(totalClicks);
        if (statsKilledText != null) statsKilledText.text = "Monsters Killed: " + FormatNumbers(monstersKilled);
        if (statsEarnedText != null) statsEarnedText.text = "Total Earned: " + FormatNumbers(totalCoinsEarned);
        if (statsSpentText != null) statsSpentText.text = "Total Spent: " + FormatNumbers(totalCoinsSpent);
        if (statsDamageText != null) statsDamageText.text = "Total Damage: " + FormatNumbers(totalDamageDone);
        if (statsGemsText != null) statsGemsText.text = "Total Gems: " + FormatNumbers(totalGemsEarned);

        if (mineUpgradePriceText != null) mineUpgradePriceText.text = "Price: " + FormatNumbers(mineUpgradePrice);
        if (upgradePriceText != null) upgradePriceText.text = "Price: " + FormatNumbers(upgradePrice);
        if (coinUpgradePriceText != null) coinUpgradePriceText.text = "Price: " + FormatNumbers(coinUpgradePrice);
        if (critUpgradePriceText != null) critUpgradePriceText.text = "Price: " + FormatNumbers(critUpgradePrice);

        if (mineStatusText != null)
        {
            float currentProd = 0f;

            if (mineLevel > 0)
            {
                currentProd = mineBaseReward * Mathf.Pow(2.2f, mineLevel - 1) * coinMultiplier * (1f + activeCoinsBoost);
            }

            mineStatusText.text = FormatNumbers(currentProd) + " / " + mineInterval + "s";
        }
    }

    public string FormatNumbers(float value)
    {
        if (value >= 1000000) return (value / 1000000f).ToString("F2") + "M";
        if (value >= 1000) return (value / 1000f).ToString("F1") + "k";

        return value.ToString("F0");
    }
}