using UnityEngine;
using UnityEngine.UI; // <-- TOTO SME PRIDALI (aby skript videl UI komponenty)

public partial class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;

    [Header("UI Prepojenie")] // <-- TOTO SME PRIDALI
    public Slider healthSlider;

    [Header("Efekty")]
    public Color damageColor = Color.red;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Nastavíme bar na začiatku na maximum
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    void OnMouseDown()
    {
        TakeDamage(10f); // Každý klik uberie 10 HP (aby si videl pohyb na bare)
    }

    void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // AKTUALIZÁCIA BARU
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log("Medúza dostala pecku! HP: " + currentHealth);

        StopAllCoroutines();
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Medúza padla!");
        Destroy(gameObject);
    }
}