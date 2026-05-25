using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVER FUNGUJE NA: " + gameObject.name);

        if (item != null && ItemTooltip.instance != null)
        {
            ItemTooltip.instance.ShowTooltip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("HOVER EXIT: " + gameObject.name);

        if (ItemTooltip.instance != null)
        {
            ItemTooltip.instance.HideTooltip();
        }
    }
}