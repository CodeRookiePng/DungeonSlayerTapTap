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
    public GameObject hairLong;
    public GameObject beardFull;
    public GameObject beardNone;

    void Start()
    {
        // 1. Načítame uložený stav postavy
        LoadCharacter();

        // 2. Otvoríme Character Creator na začiatku
        ShowCharacterCreator();
    }

    // --- PREPÍNANIE OBRAZOVIEK ---
    public void ShowCharacterCreator() { DeactivateAllScreens(); characterCreator.SetActive(true); postava.SetActive(true); SetGlobalUI(false); }
    public void ShowDungeon() { DeactivateAllScreens(); dungeonScreen.SetActive(true); postava.SetActive(false); SetGlobalUI(true); }
    public void ShowEquipment() { DeactivateAllScreens(); equipmentScreen.SetActive(true); postava.SetActive(true); SetGlobalUI(true); }
    public void ShowLeaderboard() { DeactivateAllScreens(); leaderboardScreen.SetActive(true); SetGlobalUI(true); }
    public void ShowUpgrade() { DeactivateAllScreens(); upgradeScreen.SetActive(true); SetGlobalUI(true); }
    public void ShowShop() { DeactivateAllScreens(); shopScreen.SetActive(true); SetGlobalUI(true); }
    public void ShowMap() { DeactivateAllScreens(); mapScreen.SetActive(true); SetGlobalUI(true); }

    private void DeactivateAllScreens()
    {
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

    // Pomocná funkcia na reset vlasov
    private void ResetAllHair()
    {
        if (hairBowlcut != null) hairBowlcut.SetActive(false);
        if (hairMessy != null) hairMessy.SetActive(false);
        if (hairLong != null) hairLong.SetActive(false);
    }

    // Pomocná funkcia na reset brád
    private void ResetAllBeards()
    {
        if (beardFull != null) beardFull.SetActive(false);
        if (beardNone != null) beardNone.SetActive(false);
    }

    public void SelectHair(GameObject selectedHair)
    {
        ResetAllHair();

        if (selectedHair != null)
        {
            selectedHair.SetActive(true);

            // Uloženie: 0 = Bowlcut, 1 = Messy, 2 = Long
            if (selectedHair == hairBowlcut) PlayerPrefs.SetInt("SavedHair", 0);
            else if (selectedHair == hairMessy) PlayerPrefs.SetInt("SavedHair", 1);
            else if (selectedHair == hairLong) PlayerPrefs.SetInt("SavedHair", 2);

            PlayerPrefs.Save();
        }
    }

    public void SelectBeard(GameObject selectedBeard)
    {
        ResetAllBeards();

        if (selectedBeard != null)
        {
            selectedBeard.SetActive(true);

            // Uloženie: 0 = Full Beard, 1 = Beard None
            if (selectedBeard == beardFull) PlayerPrefs.SetInt("SavedBeard", 0);
            else if (selectedBeard == beardNone) PlayerPrefs.SetInt("SavedBeard", 1);

            PlayerPrefs.Save();
        }
    }

    public void SelectGender(GameObject selectedGender)
    {
        if (maleModel != null) maleModel.SetActive(false);
        if (babyModel != null) babyModel.SetActive(false);

        if (selectedGender != null)
        {
            selectedGender.SetActive(true);

            // Uloženie: 0 = Male, 1 = Baby
            PlayerPrefs.SetInt("SavedGender", (selectedGender == maleModel) ? 0 : 1);

            // Automatické nastavenie podľa genderu
            if (selectedGender == babyModel)
            {
                SelectHair(hairLong);
                SelectBeard(beardNone); // Baba asi bradu nechce, tak ju hneď vypneme :)
            }
            else
            {
                SelectHair(hairBowlcut);
                SelectBeard(beardFull); // Mužovi dáme na začiatok plnú bradu
            }

            PlayerPrefs.Save();
        }
    }

    private void LoadCharacter()
    {
        // 1. Načítanie pohlavia
        int genderID = PlayerPrefs.GetInt("SavedGender", 0);
        if (maleModel != null) maleModel.SetActive(genderID == 0);
        if (babyModel != null) babyModel.SetActive(genderID == 1);

        // 2. Načítanie vlasov
        ResetAllHair();
        int hairID = PlayerPrefs.GetInt("SavedHair", 0);
        if (hairID == 0 && hairBowlcut != null) hairBowlcut.SetActive(true);
        if (hairID == 1 && hairMessy != null) hairMessy.SetActive(true);
        if (hairID == 2 && hairLong != null) hairLong.SetActive(true);

        // 3. Načítanie brady
        ResetAllBeards();
        int beardID = PlayerPrefs.GetInt("SavedBeard", 0);
        if (beardID == 0 && beardFull != null) beardFull.SetActive(true);
        if (beardID == 1 && beardNone != null) beardNone.SetActive(true);
    }
}