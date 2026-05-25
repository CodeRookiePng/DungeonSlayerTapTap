using UnityEngine;

public enum ItemType
{
    Weapon,
    Helmet,
    Potion,
    Ring,
    Offheld,    // Štít
    Gloves,
    Book,
    Trinket,
    Chestplate, // NOVÉ
    Leggings,   // NOVÉ
    Boots       // NOVÉ
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Clicker Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;

    [Header("UI Vizualizácia")]
    public Sprite icon; // Malá štvorcová ikonka do inventára

    [Header("Vizuál na Postave")]
    public Sprite bodyPartSprite; // Veľký sprite, ktorý sa vykreslí priamo na postave

    [Header("Clicker Štatistiky (0.10 = +10%)")]
    [Tooltip("Zvýšenie poškodenia z každého kliknutia (napr. 0.5 = +50% dmg)")]
    public float damageBoostPercent;

    [Tooltip("Zvýšenie množstva získaných mincí z potvor a bane (napr. 1.0 = +100% coins)")]
    public float coinsBoostPercent;

    [Tooltip("Pridá extra čas v sekundách pri súboji s bossom (napr. 5.0 = +5 sekúnd)")]
    public float moreBossTimeSeconds;

    [Tooltip("Šanca na kritické poškodenie navyše k upgradu z obchodu (napr. 0.05 = +5% šanca)")]
    public float critChanceBoostPercent;
}