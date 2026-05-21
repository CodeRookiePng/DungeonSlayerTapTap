using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject dungeonScreen;
    public GameObject leaderboardScreen;
    public GameObject upgradeScreen;
    public GameObject shopScreen;    // Nový šuflík pre tvoj 9:16 Shop s mincami a váhami
    public GameObject mapScreen;     // Nový šuflík pre tvoju mapu s ostrovmi

    // Táto funkcia sa spustí pri štarte hry
    void Start()
    {
        // Na začiatku chceme vidieť Dungeon, ostatné vypneme
        ShowDungeon();
    }

    public void ShowDungeon()
    {
        DeactivateAllScreens();
        dungeonScreen.SetActive(true);
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

    /// <summary>
    /// Pomocná funkcia, ktorá bleskovo vypne úplne všetky obrazovky.
    /// Nemusíme tak písať "SetActive(false)" ručne do každej funkcie.
    /// </summary>
    private void DeactivateAllScreens()
    {
        if (dungeonScreen != null) dungeonScreen.SetActive(false);
        if (leaderboardScreen != null) leaderboardScreen.SetActive(false);
        if (upgradeScreen != null) upgradeScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
        if (mapScreen != null) mapScreen.SetActive(false);
    }
}