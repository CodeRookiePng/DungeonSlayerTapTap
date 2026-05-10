using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 1f;
    public int coinReward = 15;

    [Header("UI Prepojenie")]
    public Slider healthSlider;
    public GameObject damageTextPrefab;
    public Transform canvasTransform;

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

        if (GameManager.instance != null)
        {
            GameManager.instance.AddClick();

            // Získame základné poškodenie
            float damageToDeal = GameManager.instance.clickDamage;

            // LOGIKA PRE KRITICKÝ ZÁSAH
            float roll = Random.Range(0f, 100f);
            if (roll <= GameManager.instance.critChance)
            {
                damageToDeal *= GameManager.instance.critMultiplier;
                Debug.Log("<color=yellow>CRITICAL HIT!</color> Poškodenie: " + damageToDeal);
            }

            // Nahlásenie finálneho poškodenia do štatistík
            GameManager.instance.AddDamageStat(damageToDeal);

            TakeDamage(damageToDeal);
            SpawnDamagePopup(damageToDeal);
        }
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

    // TÚTO FUNKCIU VOLÁ GAMEMANAGER
    public void ResetEnemy()
    {
        isDead = false;
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.value = maxHealth;
            healthSlider.gameObject.SetActive(true);
        }

        spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;

        Debug.Log("Medúsa bola úspešne oživená cez ResetEnemy!");
    }

    void SpawnDamagePopup(float amount)
    {
        if (damageTextPrefab != null && canvasTransform != null)
        {
            Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position);
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvasTransform);

            textObj.GetComponent<TMPro.TextMeshProUGUI>().text = "-" + amount.ToString("F1");
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

        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill(coinReward);
            // HLAVNÁ ZMENA: Žiadosť o respawn posielame manageru
            GameManager.instance.RequestRespawn(this.gameObject, respawnTime);
        }

        // Skryjeme nepriateľa
        spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        Debug.Log("Medúsa padla, čakám na respawn od Managera...");
    }
}