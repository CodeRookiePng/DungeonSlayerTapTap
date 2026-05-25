using UnityEngine;

public class ToolbarToggle : MonoBehaviour
{
    [Header("Panel, ktorý sa má schovať/zobraziť")]
    public GameObject toolbarPanel;

    [Header("Predvolený stav pri štarte hry")]
    public bool startOpen = false;

    private void Start()
    {
        // Nastaví panel na začiatku hry podľa toho, čo zaklikneš v Inspectore
        if (toolbarPanel != null)
        {
            toolbarPanel.SetActive(startOpen);
        }
    }

    // Táto funkcia sa zavolá po kliknutí na Button
    public void TogglePanel()
    {
        if (toolbarPanel != null)
        {
            // Zoberie aktuálny stav (true/false) a otočí ho na opačný (!)
            bool currentState = toolbarPanel.activeSelf;
            toolbarPanel.SetActive(!currentState);
        }
    }
}