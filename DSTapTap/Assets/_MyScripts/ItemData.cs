using UnityEngine;

// Definuje, aké typy vybaviteľných vecí v hre máš
public enum ItemType
{
    Weapon,
    Helmet,
    Potion,
    Ring,
    Offheld,
    Gloves,
    Book,
    Trinket
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType; // Typ veci (podľa tohto skript spozná, kam ju nasadiť)
    public Sprite icon;       // Obrázok meča/helmy, ktorý si nakreslil

    // Sem môžeš neskôr pridať staty (napr. public int attack;)
}