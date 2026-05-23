using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    // Tu v Unity pretiahneš tie objekty vlasov
    public GameObject hairBowlCut;
    public GameObject hairMessy;

    // Funkcia pre ľavé tlačidlo
    public void SelectBowlCut()
    {
        hairBowlCut.SetActive(true);
        hairMessy.SetActive(false);
    }

    // Funkcia pre pravé tlačidlo
    public void SelectMessyHair()
    {
        hairBowlCut.SetActive(false);
        hairMessy.SetActive(true);
    }
}