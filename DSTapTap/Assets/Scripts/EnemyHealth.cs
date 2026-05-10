using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 3f;

    [Header("UI Prepojenie")]
    public Slider healthSlider;

    [Header("Vizuálne Efekty")]
    public Color damageColor = Color.red;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    [Header("Nastavenia Klikania")]
    private float nextDamageTime = 0f;
    public float damageCooldown = 0.2f; // Minimálna pauza medzi klikmi

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalColor = spriteRenderer.color;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }
    void OnEnable()
    {
        // Ak je medúza mŕtva v momente, keď zapneš obrazovku,
        // pre istotu reštartujeme respawn, aby tam neostala visieť navždy.
        if (isDead)
        {
            StopAllCoroutines(); // Zastavíme staré zaseknuté pokusy
            StartCoroutine(RespawnSequence());
        }
    }

    // Hlavná metóda pre kliknutie
    void OnMouseDown()
    {
        // Kontrola: Ak je mŕtva ALEBO klikáš príliš rýchlo, nerob nič
        if (isDead || Time.time < nextDamageTime) return;

        // Nastavíme čas, kedy bude možný ďalší klik
        nextDamageTime = Time.time + damageCooldown;

        TakeDamage(10f);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Aktualizácia Slidera
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        // Spustíme efekt sčervenania
        StartCoroutine(FlashEffect());

        // Kontrola smrti
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashEffect()
    {
        // Zmeníme farbu na červenú
        spriteRenderer.color = damageColor;

        // Počkáme 0.1 sekundy
        yield return new WaitForSeconds(0.1f);

        // Vrátime pôvodnú farbu
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Medúza padla!");
        StartCoroutine(RespawnSequence());
    }

    IEnumerator RespawnSequence()
    {
        // Skryjeme medúzu a vypneme klikanie
        spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        // Reset hodnôt
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = maxHealth;
            healthSlider.gameObject.SetActive(true);
        }

        // Zobrazenie medúzy
        spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
        isDead = false;

        Debug.Log("Medúza je späť!");
    }
}