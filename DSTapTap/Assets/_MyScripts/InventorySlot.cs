using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData currentItem;
    private Image myIconImage;
    private EquipmentManager equipmentManager;

    private void Awake()
    {
        myIconImage = transform.GetChild(0).GetComponent<Image>();
        equipmentManager = FindObjectOfType<EquipmentManager>();

        Button btn = gameObject.GetComponent<Button>();
        if (btn == null) btn = gameObject.AddComponent<Button>();

        btn.onClick.AddListener(OnSlotClicked);
    }

    public void SetupSlot(ItemData newItem)
    {
        currentItem = newItem;
        myIconImage.sprite = newItem.icon;
        myIconImage.gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentItem = null;
        myIconImage.sprite = null;
        myIconImage.gameObject.SetActive(false);
    }

    private void OnSlotClicked()
    {
        if (currentItem != null && equipmentManager != null)
        {
            equipmentManager.EquipItem(currentItem, this);
        }
    }
}