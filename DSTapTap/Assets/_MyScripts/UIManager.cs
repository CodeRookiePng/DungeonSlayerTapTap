using UnityEngine;
using UnityEngine.UI; // Nezabudneme na UI namespace

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject dungeonScreen;
    public GameObject leaderboardScreen;
    public GameObject upgradeScreen;
    public GameObject shopScreen;
    public GameObject mapScreen;

    [Header("Special Buttons")]
    public GameObject statsButton; // Pretiahni sem StatsButton z hierarchie

    void Start()
    {
        ShowDungeon();
    }

    public void ShowDungeon()
    {
        DeactivateAllScreens();

        dungeonScreen.SetActive(true);

        // Stats Button zapneme LEN tu
        if (statsButton != null) statsButton.SetActive(true);
    }

    public void ShowLeaderboard()
    {
        DeactivateAllScreens();
        leaderboardScreen.SetActive(true);
    }

    public void ShowUpgrade()
    {
        DeactivateAllScreens();
        upgradeScreen.SetActive(true);
    }

    public void ShowShop()
    {
        DeactivateAllScreens();
        shopScreen.SetActive(true);
    }

    public void ShowMap()
    {
        DeactivateAllScreens();
        mapScreen.SetActive(true);
    }

    private void DeactivateAllScreens()
    {
        // Najprv schováme stats button, akonáhle opúšťame dungeon
        if (statsButton != null) statsButton.SetActive(false);

        // Potom schováme všetky obrazovky
        if (dungeonScreen != null) dungeonScreen.SetActive(false);
        if (leaderboardScreen != null) leaderboardScreen.SetActive(false);
        if (upgradeScreen != null) upgradeScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
        if (mapScreen != null) mapScreen.SetActive(false);
    }
}