using UnityEngine;

public enum ItemType
{
    Weapon,
    Helmet,
    Potion,
    Ring,
    Offheld, // Štít
    Gloves,
    Book,
    Trinket,
    Chestplate, // NOVÉ
    Leggings,   // NOVÉ
    Boots       // NOVÉ
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;

    [Header("UI Vizualizácia")]
    public Sprite icon; // Malá štvorcová ikonka do inventára

    [Header("Vizuál na Postave (NOVÉ)")]
    public Sprite bodyPartSprite; // Veľký sprite, ktorý sa vykreslí priamo na postave
}