using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Umožní nám to pristupovať k tomuto skriptu odvšadiaľ

    [Header("Štatistiky")]
    public int totalClicks = 0;
    public int monstersKilled = 0;
    public int coins = 0;

    [Header("UI Texty")]
    public TextMeshProUGUI coinText; // Text pre coiny hore
    public TextMeshProUGUI statsClickText; // Text pre kliky v okne
    public TextMeshProUGUI statsKilledText; // Text pre zabitých v okne

    public GameObject statsPanel; // Okno štatistík

    void Awake()
    {
        instance = this; // Nastavenie "mozgu"
    }

    void Start()
    {
        UpdateUI();
        statsPanel.SetActive(false); // Skryjeme okno na začiatku
    }

    // Voláme pri každom kliknutí
    public void AddClick()
    {
        totalClicks++;
        UpdateUI();
    }

    // Voláme, keď Medúsa zomrie
    public void AddKill(int reward)
    {
        monstersKilled++;
        coins += reward;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (coinText != null) coinText.text = "Coins: " + coins;
        if (statsClickText != null) statsClickText.text = "Total Clicks: " + totalClicks;
        if (statsKilledText != null) statsKilledText.text = "Monsters Killed: " + monstersKilled;
    }

    // Pre tlačidlo Štatistiky
    public void ToggleStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        UpdateUI();
    }
}