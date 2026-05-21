using UnityEngine;
using TMPro;
using System;

public class ShopIAPManager : MonoBehaviour
{
    [Header("UI Reference (Odpočet času)")]
    public TextMeshProUGUI timerText;
    private TimeSpan remainingTime;

    [Header("Rewards to Give")]
    public int gemsReward = 500;
    public int coinsReward = 7000;

    [Header("Top Bar UI Text References")]
    public TextMeshProUGUI totalGemsText;  // Sem pretiahneš text, ktorý ukazuje celkové Gemy hore na obrazovke
    public TextMeshProUGUI totalCoinsText; // Sem pretiahneš text, ktorý ukazuje celkové Coiny hore na obrazovke

    void Start()
    {
        // 1. Spustíme odpočet na 5 dní
        remainingTime = new TimeSpan(5, 0, 0, 0);
        UpdateTimerText();
        InvokeRepeating(nameof(Countdown), 1f, 1f);

        // 2. Načítame a zobrazíme aktuálne peniaze hneď po zapnutí hry
        UpdateTopBarUI();
    }

    public void OnPurchaseComplete()
    {
        Debug.Log("Nákup bol ÚSPEŠNÝ! Odmeňujem hráča.");
        GiveRewards();
    }

    public void OnPurchaseFailed()
    {
        Debug.LogError("Nákup ponuky zlyhal alebo bol zrušený.");
    }

    private void GiveRewards()
    {
        // 1. Pripočítame odmeny do pamäte
        int noveGemy = PlayerPrefs.GetInt("Gems", 0) + gemsReward;
        int noveCoiny = PlayerPrefs.GetInt("Coins", 0) + coinsReward;

        PlayerPrefs.SetInt("Gems", noveGemy);
        PlayerPrefs.SetInt("Coins", noveCoiny);
        PlayerPrefs.Save();

        // 2. OKAMŽITE prekreslíme čísla na obrazovke!
        UpdateTopBarUI();

        // 3. Skryjeme celú ponuku z obchodu
        gameObject.SetActive(false);
    }

    // Funkcia, ktorá vezme čísla z pamäte a hodí ich do UI textov
    private void UpdateTopBarUI()
    {
        if (totalGemsText != null)
        {
            totalGemsText.text = PlayerPrefs.GetInt("Gems", 0).ToString();
        }

        if (totalCoinsText != null)
        {
            totalCoinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();
        }
    }

    // --- LOGIKA ODPOČTU ČASU ---
    void Countdown()
    {
        if (remainingTime.TotalSeconds > 0)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerText();
        }
        else
        {
            if (timerText != null) timerText.text = "EXPIRED";
            CancelInvoke(nameof(Countdown));
            gameObject.SetActive(false);
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = string.Format("{0}d {1:D2}h {2:D2}m {3:D2}s",
                remainingTime.Days, remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
        }
    }
}