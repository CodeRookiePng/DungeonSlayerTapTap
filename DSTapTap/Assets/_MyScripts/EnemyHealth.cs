using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 3f;
    public int coinReward = 15; // Koľko coinov dostaneš za smrť

    [Header("UI Prepojenie")]
    public Slider healthSlider;
    public GameObject damageTextPrefab; // Pre tie Damage Popupy
    public Transform canvasTransform;   // Pre tie Damage Popupy

    [Header("Vizuálne Efekty")]
    public Color damageColor = Color.red;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    [Header("Nastavenia Klikania")]
    private float nextDamageTime = 0f;
    public float damageCooldown = 0.1f;

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

    void OnMouseDown()
    {
        if (isDead || Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + damageCooldown;

        // NAHLÁSENIE KLIKU DO MANAGERU
        if (GameManager.instance != null) GameManager.instance.AddClick();

        TakeDamage(10f);
        SpawnDamagePopup(10f); // Zavoláme funkciu pre text
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Funkcia pre tie lietajúce čísla (zajtra fixneme ak nepôjdu)
    void SpawnDamagePopup(float amount)
    {
        if (damageTextPrefab != null && canvasTransform != null)
        {
            Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position);
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvasTransform);
            textObj.GetComponent<TMPro.TextMeshProUGUI>().text = "-" + amount;
            Destroy(textObj, 1f);
        }
    }

    IEnumerator FlashEffect()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Medúza padla!");

        // NAHLÁSENIE ZABITIA A COINOV DO MANAGERU
        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill(coinReward);
        }

        StartCoroutine(RespawnSequence());
    }

    IEnumerator RespawnSequence()
    {
        spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.value = maxHealth;
            healthSlider.gameObject.SetActive(true);
        }

        spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
        isDead = false;

        Debug.Log("Medúza je späť!");
    }
}