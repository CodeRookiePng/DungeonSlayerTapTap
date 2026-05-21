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

    void Start()
    {
        // 1. Spustíme odpočet na 5 dní
        remainingTime = new TimeSpan(5, 0, 0, 0);
        UpdateTimerText();
        InvokeRepeating(nameof(Countdown), 1f, 1f);
    }

    // Zavolá sa z Unity IAP Buttonu po úspešnom zaplatení (On Order Confirmed)
    public void OnPurchaseComplete()
    {
        Debug.Log("IAP Shop: Nákup bol ÚSPEŠNÝ! Odmeňujem hráča cez GameManager.");
        GiveRewards();
    }

    // Zavolá sa, ak nákup zlyhá (On Purchase Failed)
    public void OnPurchaseFailed()
    {
        Debug.LogError("IAP Shop: Nákup Starter Packu zlyhal alebo bol zrušený.");
    }

    private void GiveRewards()
    {
        // Skontrolujeme, či GameManager na scéne reálne existuje (používame tvoje priradené malé 'instance')
        if (GameManager.instance != null)
        {
            // 1. Cez tvoju originálnu funkciu pridáme Gemy (tá v GameManageri rovno aktualizuje aj štatistiky a UI)
            GameManager.instance.AddGems(gemsReward);

            // 2. Cez novú funkciu, ktorú sme pridali do GameManageru, pripíšeme Coiny
            GameManager.instance.AddCoinsFromShop(coinsReward);

            // 3. Skryjeme ponuku z obchodu, keďže už je kúpená
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("ShopIAPManager: GameManager.instance chýba na scéne! Skontroluj, či máš GameManager v scéne hodený.");
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