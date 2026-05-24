using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public ItemData testItem; // Sem v Inspectore pretiahneš tvoj súbor MystickyMec
    public InventorySlot targetSlot; // Sem pretiahneš prvý Slot z tvojho InventoryGrid

    void Start()
    {
        // Počkáme sekundu po štarte a vložíme meč do slotu
        if (testItem != null && targetSlot != null)
        {
            targetSlot.SetupSlot(testItem);
        }
    }
}