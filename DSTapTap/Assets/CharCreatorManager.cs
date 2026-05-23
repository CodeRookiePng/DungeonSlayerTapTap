using UnityEngine;

public class CharCreatorManager : MonoBehaviour
{
    [Header("Panely - Obrazovky")]
    public GameObject characterCreatorPanel;
    public GameObject dungeonScreenPanel;

    [Header("Sekcia Pohlavie")]
    public GameObject maleModel;
    public GameObject babyModel;

    [Header("Sekcia Vlasy")]
    public GameObject hairBowlcut;
    public GameObject hairMessy;

    [Header("Sekcia Brada")]
    public GameObject beardFull;
    public GameObject beardNone;

    void Start()
    {
        // Pri štarte hry vždy nastavíme správny stav
        ShowCharacterCreator();
    }

    // --- FUNKCIE PRE OBRAZOVKY ---
    public void ShowCharacterCreator()
    {
        characterCreatorPanel.SetActive(true);
        dungeonScreenPanel.SetActive(false);
    }

    public void ShowDungeonScreen()
    {
        characterCreatorPanel.SetActive(false);
        dungeonScreenPanel.SetActive(true);
    }

    // --- FUNKCIE PRE POSTAVU ---
    public void SelectGender(GameObject selectedGender)
    {
        maleModel.SetActive(false);
        babyModel.SetActive(false);
        selectedGender.SetActive(true);
    }

    public void SelectHair(GameObject selectedHair)
    {
        hairBowlcut.SetActive(false);
        hairMessy.SetActive(false);
        selectedHair.SetActive(true);
    }

    public void SelectBeard(GameObject selectedBeard)
    {
        beardFull.SetActive(false);
        beardNone.SetActive(false);
        selectedBeard.SetActive(true);
    }
}