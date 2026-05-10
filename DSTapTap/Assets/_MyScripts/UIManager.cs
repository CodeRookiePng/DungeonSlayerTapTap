using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Tu si pripravíme "šuflíky" pre tvoje obrazovky
    public GameObject dungeonScreen;
    public GameObject leaderboardScreen;
    public GameObject upgradeScreen;

    // Táto funkcia sa spustí pri štarte hry
    void Start()
    {
        // Na začiatku chceme vidieť Dungeon, ostatné vypneme
        ShowDungeon();
    }

    public void ShowDungeon()
    {
        dungeonScreen.SetActive(true);
        leaderboardScreen.SetActive(false);
        upgradeScreen.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        dungeonScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        upgradeScreen.SetActive(false);
    }

    public void ShowUpgrade()
    {
        dungeonScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        upgradeScreen.SetActive(true);
    }
}