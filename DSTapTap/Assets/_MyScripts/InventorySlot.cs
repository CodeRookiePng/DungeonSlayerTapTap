using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData currentItem; // Aký item momentálne leží v tomto slote
    private Image myIconImage;   // Odkaz na to biele "Icon" dieťa vo vnútri slotu
    private EquipmentManager equipmentManager;

    private void Awake()
    {
        // Automaticky si nájde komponent Image v pod-objekte (Icon)
        myIconImage = transform.GetChild(0).GetComponent<Image>();
        equipmentManager = FindObjectOfType<EquipmentManager>();

        // Na tento slot pridáme komponent Button cez kód, aby bol klikateľný
        Button btn = gameObject.GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();

        // Nastavíme, čo sa stane pri kliknutí na slot
        btn.onClick.AddListener(OnSlotClicked);
    }

    // Funkcia na vloženie itemu do inventára (napr. pri loote z dungeonu)
    public void SetupSlot(ItemData newItem)
    {
        currentItem = newItem;
        myIconImage.sprite = newItem.icon;
        myIconImage.gameObject.SetActive(true);
    }

    // Vycistenie slotu po tom, čo vec nasadíme na postavu
    public void ClearSlot()
    {
        currentItem = null;
        myIconImage.sprite = null;
        myIconImage.gameObject.SetActive(false);
    }

    // Volá sa pri kliknutí prstom na slot
    private void OnSlotClicked()
    {
        if (currentItem != null)
        {
            // Povieme manageru, nech tento item presunie hore
            equipmentManager.EquipItem(currentItem, this);
        }
    }
}