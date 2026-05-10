using UnityEngine;

public partial class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Efekty")]
    public Color damageColor = Color.red;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Táto funkcia sa zavolá automaticky, keď klikneš na objekt s Colliderom
    void OnMouseDown()
    {
        TakeDamage(10f); // Každý klik uberie 10 HP
    }

    void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Medúza dostala pecku! HP: " + currentHealth);

        // Vizuálna odozva - bliknutie na červeno
        StopAllCoroutines();
        StartCoroutine(FlashEffect());

        // Kontrola smrti
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
        // Tu môžeš pridať časticové efekty alebo animáciu
        Destroy(gameObject);
    }
}