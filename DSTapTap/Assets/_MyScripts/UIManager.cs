using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panely / Obrazovky")]
    public GameObject characterCreator;
    public GameObject dungeonScreen;
    public GameObject equipmentScreen;
    public GameObject leaderboardScreen;
    public GameObject upgradeScreen;
    public GameObject shopScreen;
    public GameObject mapScreen;

    [Header("Hlavný Objekt Postavy (mimo panelov)")]
    public GameObject postava;

    [Header("Globálne UI")]
    public GameObject navBar;
    public GameObject statsButton;
    public GameObject coinImage;

    [Header("Modely Vzhľadu (deti objektu Postava)")]
    public GameObject maleModel;
    public GameObject babyModel;
    public GameObject hairBowlcut;
    public GameObject hairMessy;
    public GameObject beardFull;
    public GameObject beardNone;

    void Start()
    {
        // 1. Načítame, čo mal hráč vybraté (vlasy, brada...)
        LoadCharacter();

        // 2. Otvoríme Character Creator na začiatku
        ShowCharacterCreator();
    }

    // --- PREPÍNANIE OBRAZOVIEK ---

    public void ShowCharacterCreator()
    {
        DeactivateAllScreens();
        characterCreator.SetActive(true);
        postava.SetActive(true); // Postavu chceme vidieť, keď ju tvoríme
        SetGlobalUI(false);      // Skryjeme NavBar a veci okolo
    }

    public void ShowDungeon()
    {
        DeactivateAllScreens();
        dungeonScreen.SetActive(true);
        postava.SetActive(false); // V dungeone postavu (2D modely) zatiaľ schováme, ak tam nehrá rolku
        SetGlobalUI(true);
    }

    public void ShowEquipment()
    {
        DeactivateAllScreens();
        equipmentScreen.SetActive(true);
        postava.SetActive(true); // TU JE TO DÔLEŽITÉ: Zapneme ju, aby stála v Equipmente!
        SetGlobalUI(true);
    }

    public void ShowLeaderboard() { DeactivateAllScreens(); leaderboardScreen.SetActive(true); SetGlobalUI(true); }
    public void ShowUpgrade() { DeactivateAllScreens(); upgradeScreen.SetActive(true); SetGlobalUI(true); }
    public void ShowShop() { DeactivateAllScreens(); shopScreen.SetActive(true); SetGlobalUI(true); }
    public void ShowMap() { DeactivateAllScreens(); mapScreen.SetActive(true); SetGlobalUI(true); }


    // --- POMOCNÉ FUNKCIE ---

    private void DeactivateAllScreens()
    {
        // Vypíname LEN panely. Objekt "postava" tu nevypíname!
        if (characterCreator != null) characterCreator.SetActive(false);
        if (dungeonScreen != null) dungeonScreen.SetActive(false);
        if (equipmentScreen != null) equipmentScreen.SetActive(false);
        if (leaderboardScreen != null) leaderboardScreen.SetActive(false);
        if (upgradeScreen != null) upgradeScreen.SetActive(false);
        if (shopScreen != null) shopScreen.SetActive(false);
        if (mapScreen != null) mapScreen.SetActive(false);
    }

    private void SetGlobalUI(bool active)
    {
        if (navBar != null) navBar.SetActive(active);
        if (statsButton != null) statsButton.SetActive(active);
        if (coinImage != null) coinImage.SetActive(active);
    }


    // --- SYSTÉM VÝBERU A UKLADANIA (PlayerPrefs) ---

    public void SelectHair(GameObject selectedHair)
    {
        hairBowlcut.SetActive(false);
        hairMessy.SetActive(false);
        selectedHair.SetActive(true);

        // Uloženie: 0 = Bowlcut, 1 = Messy
        PlayerPrefs.SetInt("SavedHair", (selectedHair == hairBowlcut) ? 0 : 1);
        PlayerPrefs.Save();
    }

    public void SelectGender(GameObject selectedGender)
    {
        maleModel.SetActive(false);
        babyModel.SetActive(false);
        selectedGender.SetActive(true);

        // Uloženie: 0 = Male, 1 = Baby
        PlayerPrefs.SetInt("SavedGender", (selectedGender == maleModel) ? 0 : 1);
        PlayerPrefs.Save();
    }

    private void LoadCharacter()
    {
        // Načítanie vlasov (ak kľúč neexistuje, dá default 0)
        int hairID = PlayerPrefs.GetInt("SavedHair", 0);
        if (hairBowlcut != null) hairBowlcut.SetActive(hairID == 0);
        if (hairMessy != null) hairMessy.SetActive(hairID == 1);

        // Načítanie pohlavia
        int genderID = PlayerPrefs.GetInt("SavedGender", 0);
        if (maleModel != null) maleModel.SetActive(genderID == 0);
        if (babyModel != null) babyModel.SetActive(genderID == 1);
    }
}