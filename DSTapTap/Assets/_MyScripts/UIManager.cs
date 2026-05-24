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

    [Header("Farba Pleti (Mužské Skiny / Spoločné Skiny)")]
    public GameObject skinWhite;
    public GameObject skinCaucasian;
    public GameObject skinBrown;
    public GameObject skinBlack;

    void Start()
    {
        LoadCharacter();
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

    // --- RESET FUNKCIE ---
    private void ResetAllHair()
    {
        if (hairBowlcut != null) hairBowlcut.SetActive(false);
        if (hairMessy != null) hairMessy.SetActive(false);
        if (hairLong != null) hairLong.SetActive(false);
    }

    private void ResetAllBeards()
    {
        if (beardFull != null) beardFull.SetActive(false);
        if (beardNone != null) beardNone.SetActive(false);
    }

    private void ResetAllSkins()
    {
        if (skinWhite != null) skinWhite.SetActive(false);
        if (skinCaucasian != null) skinCaucasian.SetActive(false);
        if (skinBrown != null) skinBrown.SetActive(false);
        if (skinBlack != null) skinBlack.SetActive(false);
    }

    // --- SYSTÉM VÝBERU A UKLADANIA ---

    public void SelectHair(GameObject selectedHair)
    {
        ResetAllHair();
        if (selectedHair != null)
        {
            selectedHair.SetActive(true);
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
            if (selectedBeard == beardFull) PlayerPrefs.SetInt("SavedBeard", 0);
            else if (selectedBeard == beardNone) PlayerPrefs.SetInt("SavedBeard", 1);
            PlayerPrefs.Save();
        }
    }

    public void SelectSkinColor(GameObject selectedSkin)
    {
        // Ak je aktívna žena (babyModel), a tieto skiny sú len mužské, 
        // tak stlačenie tlačidla farby pleti pre muža zablokujeme, aby jej neskákalo do textúry.
        if (babyModel != null && babyModel.activeSelf)
        {
            // Ak máš pre ženu rovnaké skiny, tento "return" vymaž. 
            // Ak sú skiny len mužské, toto zabráni tomu, aby mužská koža prekryla ženu.
            return;
        }

        ResetAllSkins();
        if (selectedSkin != null)
        {
            selectedSkin.SetActive(true);

            if (selectedSkin == skinWhite) PlayerPrefs.SetInt("SavedSkin", 0);
            else if (selectedSkin == skinCaucasian) PlayerPrefs.SetInt("SavedSkin", 1);
            else if (selectedSkin == skinBrown) PlayerPrefs.SetInt("SavedSkin", 2);
            else if (selectedSkin == skinBlack) PlayerPrefs.SetInt("SavedSkin", 3);

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
            PlayerPrefs.SetInt("SavedGender", (selectedGender == maleModel) ? 0 : 1);

            if (selectedGender == babyModel)
            {
                // !!! TU JE TA OPRAVA !!!
                // Keď klikneš na babu, kompletne vypneme všetky mužské farby pleti, aby ju neprekrývali
                ResetAllSkins();

                SelectHair(hairLong);
                SelectBeard(beardNone);
            }
            else
            {
                // Keď klikneš na muža, vrátime mu jeho základnú kožu (White)
                SelectSkinColor(skinWhite);
                SelectHair(hairBowlcut);
                SelectBeard(beardFull);
            }
            PlayerPrefs.Save();
        }
    }

    private void LoadCharacter()
    {
        int genderID = PlayerPrefs.GetInt("SavedGender", 0);
        if (maleModel != null) maleModel.SetActive(genderID == 0);
        if (babyModel != null) babyModel.SetActive(genderID == 1);

        ResetAllHair();
        int hairID = PlayerPrefs.GetInt("SavedHair", 0);
        if (hairID == 0 && hairBowlcut != null) hairBowlcut.SetActive(true);
        if (hairID == 1 && hairMessy != null) hairMessy.SetActive(true);
        if (hairID == 2 && hairLong != null) hairLong.SetActive(true);

        ResetAllBeards();
        int beardID = PlayerPrefs.GetInt("SavedBeard", 0);
        if (beardID == 0 && beardFull != null) beardFull.SetActive(true);
        if (beardID == 1 && beardNone != null) beardNone.SetActive(true);

        // Načítanie pleti (len ak nie je zapnutá baba, alebo ak sú skiny spoločné)
        ResetAllSkins();
        if (genderID == 0) // Ak je to muž (ID 0), načítame mu jeho kožu
        {
            int skinID = PlayerPrefs.GetInt("SavedSkin", 0);
            if (skinID == 0 && skinWhite != null) skinWhite.SetActive(true);
            if (skinID == 1 && skinCaucasian != null) skinCaucasian.SetActive(true);
            if (skinID == 2 && skinBrown != null) skinBrown.SetActive(true);
            if (skinID == 3 && skinBlack != null) skinBlack.SetActive(true);
        }
    }
}