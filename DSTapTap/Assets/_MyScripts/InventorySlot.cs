using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Komponenty Slotu")]
    public Image iconImage;

    [Header("Aktuálny Item v Slote")]
    public ItemData currentItem;

    private void Awake()
    {
        if (iconImage == null)
        {
            iconImage = GetComponentInChildren<Image>();
        }
    }

    private void Start()
    {
        // AUTOMATICKÉ PREPOJENIE: Skript si sám nájde komponent Button na tomto slote
        Button btn = GetComponent<Button>();
        if (btn == null)
        {
            // Ak na objekte náhodou chýba Button, skript ho pre istotu sám pridá
            btn = gameObject.AddComponent<Button>();
        }

        // Priradíme funkciu kliknutia priamo cez kód
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnSlotClicked);

        // Inicializácia vizuálu
        if (currentItem != null) SetupSlot(currentItem);
        else ClearSlot();
    }

    public void SetupSlot(ItemData newItem)
    {
        if (newItem == null)
        {
            ClearSlot();
            return;
        }

        currentItem = newItem;

        if (iconImage != null)
        {
            iconImage.sprite = currentItem.icon;
            iconImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void ClearSlot()
    {
        currentItem = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1f, 1f, 1f, 0f);
        }

        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.ShowTooltip(currentItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.HideTooltip();
        }
    }

    private void OnDisable()
    {
        if (ItemTooltip.Instance != null) ItemTooltip.Instance.HideTooltip();
    }

    public void OnSlotClicked()
    {
        if (currentItem != null)
        {
            EquipmentManager eqManager = FindObjectOfType<EquipmentManager>();
            if (eqManager != null)
            {
                eqManager.EquipItem(currentItem, this);
            }
        }
    }
}