using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    [Header("UI Referencie (Pretiahni TMP texty z Hierarchy)")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public TextMeshProUGUI itemStatsText;

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        // Na začiatku hry tooltip skryjeme
        HideTooltip();
    }

    private void Update()
    {
        if (canvas == null || rectTransform == null) return;

        // Tooltip plynule nasleduje kurzor myši
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePosition
        );

        // Jemný posun (offset), aby tooltip nebol priamo pod kurzorom
        rectTransform.anchoredPosition = mousePosition + new Vector2(15f, -15f);
    }

    public void ShowTooltip(ItemData item)
    {
        if (item == null) return;

        // POISTKA PROTI CHYBE: Ak chýbajú referencie v Unity, skript nespadne
        if (itemNameText == null || itemTypeText == null || itemStatsText == null)
        {
            Debug.LogWarning("[ItemTooltip] POZOR! Nemáš priradené textové objekty v Inspectore na objekte ItemTooltip! Tooltip nemôže vypísať text.");
            gameObject.SetActive(true);
            return;
        }

        gameObject.SetActive(true);

        // Nastavenie textov
        itemNameText.text = item.itemName;
        itemTypeText.text = item.itemType.ToString();

        // Dynamické skladanie štatistík
        string statsString = "";

        if (item.damageBoostPercent > 0f)
            statsString += $"<color=#FF5555>Click Damage: +{item.damageBoostPercent * 100f:F0}%</color>\n";

        if (item.coinsBoostPercent > 0f)
            statsString += $"<color=#FFD700>Gold Boost: +{item.coinsBoostPercent * 100f:F0}%</color>\n";

        if (item.moreBossTimeSeconds > 0f)
            statsString += $"<color=#55FF55>Boss Time: +{item.moreBossTimeSeconds:F0}s</color>\n";

        if (item.critChanceBoostPercent > 0f)
            statsString += $"<color=#FF9900>Crit Chance: +{item.critChanceBoostPercent * 100f:F0}%</color>\n";

        if (string.IsNullOrEmpty(statsString))
            statsString = "<color=#888888>No bonus stats</color>";

        itemStatsText.text = statsString;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}