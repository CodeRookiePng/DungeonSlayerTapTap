using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 1f;
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
        // Kontrola, aby sme neklikali na mŕtvolu alebo príliš rýchlo
        if (isDead || Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + damageCooldown;

        // Nahlásenie kliku do štatistík
        if (GameManager.instance != null)
            GameManager.instance.AddClick();

        // Získanie poškodenia z GameManageru (to, čo si si vylepšil)
        float currentDmg = (GameManager.instance != null) ? GameManager.instance.clickDamage : 10f;

        TakeDamage(currentDmg);
        SpawnDamagePopup(currentDmg);
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

    void SpawnDamagePopup(float amount)
    {
        if (damageTextPrefab != null && canvasTransform != null)
        {
            Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position);
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvasTransform);

            // Zaokrúhlime číslo na 1 desatinné miesto, aby tam nebolo 15.0000001
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

        Debug.Log("Medúza padla!");

        // Odmena za zabitie
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