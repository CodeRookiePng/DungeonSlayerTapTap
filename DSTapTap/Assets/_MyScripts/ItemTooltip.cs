using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip instance;

    [Header("UI")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemStatsText;

    void Awake()
    {
        instance = this;

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("[ItemTooltip] Item je NULL!");
            return;
        }

        if (tooltipPanel == null)
        {
            Debug.LogWarning("[ItemTooltip] TooltipPanel nie je priradený!");
            return;
        }

        tooltipPanel.SetActive(true);

        if (itemNameText != null)
            itemNameText.text = item.itemName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        string stats = "";

        if (item.damageBoostPercent > 0)
            stats += $"Damage: +{item.damageBoostPercent * 100}%\n";

        if (item.coinsBoostPercent > 0)
            stats += $"Coins: +{item.coinsBoostPercent * 100}%\n";

        if (item.moreBossTimeSeconds > 0)
            stats += $"Boss Time: +{item.moreBossTimeSeconds}s\n";

        if (item.critChanceBoostPercent > 0)
            stats += $"Crit Chance: +{item.critChanceBoostPercent * 100}%\n";

        if (string.IsNullOrEmpty(stats))
            stats = "Žiadne bonusy";

        if (itemStatsText != null)
            itemStatsText.text = stats;

        Debug.Log("[ItemTooltip] Zobrazujem tooltip pre item: " + item.itemName);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        Debug.Log("[ItemTooltip] Tooltip schovaný");
    }
}