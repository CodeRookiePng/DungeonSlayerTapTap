using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Boss Systém")]
    public int killsToBoss = 10;
    public int currentKills = 0;
    public TextMeshProUGUI bossProgressText;
    public bool nextIsBoss = false;
    public float bossTimeLimit = 15f; // Časový limit na bossa

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
        UpdateUI();
        if (statsPanel != null) statsPanel.SetActive(false);
        StartCoroutine(GoldMineRoutine());
        UpdateBossUI();
    }

    // --- GOLD MINE LOGIKA ---
    private IEnumerator GoldMineRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(mineInterval);
            if (mineLevel > 0)
            {
                float baseReward = mineBaseReward * Mathf.Pow(2.2f, mineLevel - 1);
                int finalReward = Mathf.RoundToInt(baseReward * coinMultiplier);

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

    // --- LOGIKA HRY ---
    public void AddClick() { totalClicks++; UpdateUI(); }
    public void AddDamageStat(float amount) { totalDamageDone += amount; UpdateUI(); }

    public void AddKill(int baseReward)
    {
        monstersKilled++;

        int finalReward = Mathf.RoundToInt(baseReward * coinMultiplier);
        coins += finalReward;
        totalCoinsEarned += finalReward;

        if (nextIsBoss)
        {
            currentKills = 0;
            nextIsBoss = false;
            Debug.Log("<color=green>BOSS PORAZENÝ!</color>");
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

    // FUNKCIA PRE ZLYHANIE PRI BOSSOVI
    public void BossFailed(GameObject boss)
    {
        Debug.Log("<color=orange>ČAS VYPRŠAL! Boss utiekol.</color>");
        currentKills = 0;
        nextIsBoss = false;

        UpdateBossUI();

        // Okamžitý reset nepriateľa na bežnú verziu
        if (boss != null)
        {
            var healthScript = boss.GetComponent<EnemyHealth>();
            if (healthScript != null) healthScript.ResetEnemy();
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
            critUpgradePrice = Mathf.RoundToInt(critUpgradePrice * 1.3f);
            UpdateUI();
        }
    }

    // --- RESPANN LOGIKA ---
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
            if (healthScript != null) healthScript.ResetEnemy();
        }
    }

    // --- UI ---
    public void ToggleStats()
    {
        if (statsPanel != null) { statsPanel.SetActive(!statsPanel.activeSelf); UpdateUI(); }
    }

    public void UpdateUI()
    {
        if (coinText != null) coinText.text = FormatNumbers(coins);

        if (statsClickText != null) statsClickText.text = "Total Clicks: " + FormatNumbers(totalClicks);
        if (statsKilledText != null) statsKilledText.text = "Monsters Killed: " + FormatNumbers(monstersKilled);
        if (statsEarnedText != null) statsEarnedText.text = "Total Earned: " + FormatNumbers(totalCoinsEarned);
        if (statsSpentText != null) statsSpentText.text = "Total Spent: " + FormatNumbers(totalCoinsSpent);
        if (statsDamageText != null) statsDamageText.text = "Total Damage: " + FormatNumbers(totalDamageDone);

        if (mineUpgradePriceText != null) mineUpgradePriceText.text = "Price: " + FormatNumbers(mineUpgradePrice);
        if (upgradePriceText != null) upgradePriceText.text = "Price: " + FormatNumbers(upgradePrice);
        if (coinUpgradePriceText != null) coinUpgradePriceText.text = "Price: " + FormatNumbers(coinUpgradePrice);
        if (critUpgradePriceText != null) critUpgradePriceText.text = "Price: " + FormatNumbers(critUpgradePrice);

        if (mineStatusText != null)
        {
            float currentProd = (mineLevel == 0) ? 0 : (mineBaseReward * Mathf.Pow(2.2f, mineLevel - 1)) * coinMultiplier;
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