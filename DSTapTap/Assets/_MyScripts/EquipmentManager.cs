using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    [Header("Horné Equipment Sloty")]
    public Image slotWeaponIcon;
    public Image slotHelmetIcon;
    public Image slotPotionIcon;
    public Image slotRingIcon;
    public Image slotOffheldIcon;
    public Image slotGlovesIcon;
    public Image slotBookIcon;
    public Image slotTrinketIcon;

    // Sem si uložíme dáta o tom, aký item reálne na sebe postava má
    private Dictionary<ItemType, ItemData> equippedItems = new Dictionary<ItemType, ItemData>();
    private Dictionary<ItemType, Image> equipmentIcons;

    private void Awake()
    {
        equipmentIcons = new Dictionary<ItemType, Image>()
        {
            { ItemType.Weapon, slotWeaponIcon },
            { ItemType.Helmet, slotHelmetIcon },
            { ItemType.Potion, slotPotionIcon },
            { ItemType.Ring, slotRingIcon },
            { ItemType.Offheld, slotOffheldIcon },
            { ItemType.Gloves, slotGlovesIcon },
            { ItemType.Book, slotBookIcon },
            { ItemType.Trinket, slotTrinketIcon }
        };

        // Naštartujeme prázdne dáta pre každý typ
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            equippedItems[type] = null;
        }

        // Automaticky pridáme klikateľnosť na horné ikony
        SetupClickListeners();
    }

    public void EquipItem(ItemData item, InventorySlot sourceSlot)
    {
        if (item == null) return;

        // Ak už hore nejakú zbraň máme, najprv ju vrátime dole (vymeníme ich)
        if (equippedItems[item.itemType] != null)
        {
            UnequipItem(item.itemType);
        }

        // Nasadíme nový item
        if (equipmentIcons.TryGetValue(item.itemType, out Image targetIcon))
        {
            equippedItems[item.itemType] = item;
            targetIcon.sprite = item.icon;
            targetIcon.gameObject.SetActive(true);

            sourceSlot.ClearSlot();
        }
    }

    // Funkcia na ZLOŽENIE itemu z postavy
    public void UnequipItem(ItemType type)
    {
        ItemData itemToReturn = equippedItems[type];
        if (itemToReturn == null) return;

        // Nájdeme prvý voľný slot v dolnom inventári
        InventorySlot freeSlot = FindFirstFreeInventorySlot();

        if (freeSlot != null)
        {
            // Vrátime vec do inventára
            freeSlot.SetupSlot(itemToReturn);

            // Vyčistíme horný slot
            equippedItems[type] = null;
            equipmentIcons[type].sprite = null;
            equipmentIcons[type].gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Inventár je plný! Nemôžeš zbraň zložiť.");
        }
    }

    // Pomocná funkcia, ktorá prečeše všetkých 18 slotov a nájde prázdny
    private InventorySlot FindFirstFreeInventorySlot()
    {
        InventorySlot[] allSlots = FindObjectsOfType<InventorySlot>();

        // Zoradíme ich podľa mena alebo pozície, aby to išlo pekne od prvého po posledný
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

    // Automaticky premení tvoje horné Icon objekty na klikateľné tlačidlá
    private void SetupClickListeners()
    {
        foreach (var pair in equipmentIcons)
        {
            ItemType currentType = pair.Key;
            Image iconImage = pair.Value;

            if (iconImage != null)
            {
                // Získame parenta (celý slot, napr. Slot_Weapon), aby sa lepšie klikalo
                GameObject slotParent = iconImage.transform.parent.gameObject;

                Button btn = slotParent.GetComponent<Button>();
                if (btn == null) btn = slotParent.AddComponent<Button>();

                btn.onClick.AddListener(() => UnequipItem(currentType));
            }
        }
    }
}