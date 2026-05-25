using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    [Header("Horné Equipment Sloty (Pretiahni sem objekty Icon)")]
    public Image slotWeaponIcon;
    public Image slotHelmetIcon;
    public Image slotPotionIcon;
    public Image slotRingIcon;
    public Image slotOffheldIcon;
    public Image slotGlovesIcon;
    public Image slotBookIcon;
    public Image slotTrinketIcon;
    public Image slotChestplateIcon; // NOVÉ
    public Image slotLeggingsIcon;   // NOVÉ
    public Image slotBootsIcon;      // NOVÉ

    [Header("Vizuálne Časti na Postave (SpriteRenderers - NOVÉ)")]
    public SpriteRenderer weaponVisual;
    public SpriteRenderer helmetVisual;
    public SpriteRenderer offheldVisual; // Štít
    public SpriteRenderer glovesVisual;
    public SpriteRenderer chestplateVisual; // Trup postavy
    public SpriteRenderer leggingsVisual;   // Nohy postavy
    public SpriteRenderer bootsVisual;      // Topánky postavy

    private Dictionary<ItemType, ItemData> equippedItems = new Dictionary<ItemType, ItemData>();
    private Dictionary<ItemType, Image> equipmentIcons;
    private Dictionary<ItemType, SpriteRenderer> equipmentVisuals; // NOVÉ

    private void Awake()
    {
        // 1. Mapovanie UI ikoniek
        equipmentIcons = new Dictionary<ItemType, Image>()
        {
            { ItemType.Weapon, slotWeaponIcon },
            { ItemType.Helmet, slotHelmetIcon },
            { ItemType.Potion, slotPotionIcon },
            { ItemType.Ring, slotRingIcon },
            { ItemType.Offheld, slotOffheldIcon },
            { ItemType.Gloves, slotGlovesIcon },
            { ItemType.Book, slotBookIcon },
            { ItemType.Trinket, slotTrinketIcon },
            { ItemType.Chestplate, slotChestplateIcon }, // NOVÉ
            { ItemType.Leggings, slotLeggingsIcon },     // NOVÉ
            { ItemType.Boots, slotBootsIcon }            // NOVÉ
        };

        // 2. Mapovanie reálnych sprite-ov na postave (NOVÉ)
        equipmentVisuals = new Dictionary<ItemType, SpriteRenderer>()
        {
            { ItemType.Weapon, weaponVisual },
            { ItemType.Helmet, helmetVisual },
            { ItemType.Offheld, offheldVisual },
            { ItemType.Gloves, glovesVisual },
            { ItemType.Chestplate, chestplateVisual },
            { ItemType.Leggings, leggingsVisual },
            { ItemType.Boots, bootsVisual }
            // Potion, Ring, Book a Trinket nemajú vizuál na postave, zostanú null
        };

        // Inicializácia prázdnych slotov
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            equippedItems[type] = null;
        }

        SetupClickListeners();
    }

    // Táto funkcia vracia zoznam nasadených vecí (potrebné pre GameManager)
    public Dictionary<ItemType, ItemData> GetEquippedItems()
    {
        return equippedItems;
    }

    public void EquipItem(ItemData item, InventorySlot sourceSlot)
    {
        if (item == null) return;

        // Ak už je niečo na danom slote oblečené, najprv to vyzlečieme
        if (equippedItems[item.itemType] != null)
        {
            UnequipItem(item.itemType);
        }

        // Uložíme predmet do manažéra
        equippedItems[item.itemType] = item;

        // AKTUALIZÁCIA UI IKONY
        if (equipmentIcons.TryGetValue(item.itemType, out Image targetIcon))
        {
            if (targetIcon != null)
            {
                targetIcon.sprite = item.icon;
                targetIcon.gameObject.SetActive(true);
            }
        }

        // AKTUALIZÁCIA VIZUÁLU POSTAVY (NOVÉ)
        if (equipmentVisuals.TryGetValue(item.itemType, out SpriteRenderer targetVisual))
        {
            // Pozor: V ItemData.cs musíš mať zadefinovanú premennú public Sprite bodyPartSprite;
            if (targetVisual != null && item.bodyPartSprite != null)
            {
                targetVisual.sprite = item.bodyPartSprite;
                targetVisual.enabled = true; // Zapneme renderer, aby bol vidieť
            }
        }

        sourceSlot.ClearSlot();

        // PREPOJENIE NA GAMEMANAGER: Aktivácia % bonusov predmetu
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateActiveModifiers();
        }
    }

    public void UnequipItem(ItemType type)
    {
        ItemData itemToReturn = equippedItems[type];
        if (itemToReturn == null) return;

        InventorySlot freeSlot = FindFirstFreeInventorySlot();

        if (freeSlot != null)
        {
            freeSlot.SetupSlot(itemToReturn);
            equippedItems[type] = null;

            // VYČISTENIE UI IKONY
            if (equipmentIcons.TryGetValue(type, out Image targetIcon))
            {
                if (targetIcon != null)
                {
                    targetIcon.sprite = null;
                    targetIcon.gameObject.SetActive(false);
                }
            }

            // VYČISTENIE VIZUÁLU POSTAVY (NOVÉ)
            if (equipmentVisuals.TryGetValue(type, out SpriteRenderer targetVisual))
            {
                if (targetVisual != null)
                {
                    targetVisual.sprite = null;
                    targetVisual.enabled = false; // Vypneme renderer, keď nič nenesie
                }
            }

            // PREPOJENIE NA GAMEMANAGER: Deaktivácia % bonusov predmetu
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateActiveModifiers();
            }
        }
        else
        {
            Debug.LogWarning("Inventár je plný!");
        }
    }

    private InventorySlot FindFirstFreeInventorySlot()
    {
        // OPRAVENÉ: Hľadáme správny skript (InventorySlot) a ukladáme ho priamo do premennej allSlots
        InventorySlot[] allSlots = FindObjectsOfType<InventorySlot>();
        System.Array.Sort(allSlots, (a, b) => a.gameObject.name.CompareTo(b.gameObject.name));

        foreach (InventorySlot slot in allSlots)
        {
            if (slot.currentItem == null)
            {
                return slot;
            }
        }
        return null;
    }

    private void SetupClickListeners()
    {
        foreach (var pair in equipmentIcons)
        {
            ItemType currentType = pair.Key;
            Image iconImage = pair.Value;

            if (iconImage != null)
            {
                GameObject slotParent = iconImage.transform.parent.gameObject;
                Button btn = slotParent.GetComponent<Button>();
                if (btn == null) btn = slotParent.AddComponent<Button>();

                btn.onClick.RemoveAllListeners(); // Ochrana pred duplicitnými listenerami
                btn.onClick.AddListener(() => UnequipItem(currentType));
            }
        }
    }
}