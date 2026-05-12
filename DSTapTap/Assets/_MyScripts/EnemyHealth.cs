using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 0.3f;
    public int coinReward = 15;

    private float baseMaxHealth;
    private int baseCoinReward;
    private Vector3 originalScale;

    [Header("UI Prepojenie")]
    public Slider healthSlider;
    public GameObject damageTextPrefab;
    public Transform canvasTransform;

    [Header("Vizuálne Efekty")]
    public Color damageColor = Color.red;
    public GameObject damageVFXPrefab;
    public GameObject critDamageVFXPrefab;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    [Header("Nastavenia Klikania")]
    private float nextDamageTime = 0f;
    public float damageCooldown = 0.1f;

    [Header("Boss Timer")]
    private Coroutine bossTimerCoroutine;
    private bool isBoss = false;

    private bool isDead = false;

    void Start()
    {
        baseMaxHealth = maxHealth;
        baseCoinReward = coinReward;
        originalScale = transform.localScale;

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
            float damageToDeal = GameManager.instance.clickDamage;
            bool isCrit = false;

            if (Random.Range(0f, 100f) <= GameManager.instance.critChance)
            {
                damageToDeal *= GameManager.instance.critMultiplier;
                isCrit = true;
            }

            GameManager.instance.AddDamageStat(damageToDeal);
            TakeDamage(damageToDeal);
            SpawnVFXAtMouse(isCrit);
            SpawnDamagePopup(damageToDeal, isCrit);
        }
    }

    public void SpawnVFXAtMouse(bool isCrit)
    {
        GameObject vfxToSpawn = isCrit ? critDamageVFXPrefab : damageVFXPrefab;
        if (vfxToSpawn == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = -1f;

        Instantiate(vfxToSpawn, worldPos, Quaternion.identity);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        StartCoroutine(FlashEffect());

        if (currentHealth <= 0) Die();
    }

    public void ResetEnemy()
    {
        isDead = false;

        if (bossTimerCoroutine != null) StopCoroutine(bossTimerCoroutine);

        if (GameManager.instance != null && GameManager.instance.nextIsBoss)
        {
            isBoss = true;
            maxHealth = baseMaxHealth * 5f;
            coinReward = baseCoinReward * 10;
            transform.localScale = originalScale * 1.3f;
            spriteRenderer.color = new Color(1f, 0.6f, 0.6f);

            // Spustíme vylepšený časovač
            bossTimerCoroutine = StartCoroutine(BossTimer(GameManager.instance.bossTimeLimit));
        }
        else
        {
            isBoss = false;
            maxHealth = baseMaxHealth;
            coinReward = baseCoinReward;
            transform.localScale = originalScale;
            spriteRenderer.color = new Color(1f, 1f, 1f);
        }

        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            healthSlider.gameObject.SetActive(true);
        }

        spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
    }

    // --- VYLEPŠENÝ ČASOVAČ S ODPOČTOM ---
    private IEnumerator BossTimer(float timeLimit)
    {
        float timeLeft = timeLimit;

        while (timeLeft > 0)
        {
            if (GameManager.instance != null && GameManager.instance.bossProgressText != null)
            {
                // Aktualizujeme UI text každú desatinu sekundy
                GameManager.instance.bossProgressText.text = "TIME: " + timeLeft.ToString("F1") + "s";
            }

            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }

        if (!isDead && isBoss)
        {
            GameManager.instance.BossFailed(this.gameObject);
        }
    }

    void SpawnDamagePopup(float amount, bool isCrit)
    {
        if (damageTextPrefab != null && canvasTransform != null)
        {
            Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position);
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvasTransform);
            var textMesh = textObj.GetComponent<TMPro.TextMeshProUGUI>();

            if (isCrit)
            {
                textMesh.color = Color.yellow;
                textMesh.text = "-" + GameManager.instance.FormatNumbers(amount) + "!";
                textMesh.fontSize *= 1.2f;
            }
            else
            {
                textMesh.text = "-" + GameManager.instance.FormatNumbers(amount);
            }

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

        if (bossTimerCoroutine != null) StopCoroutine(bossTimerCoroutine);

        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill(coinReward);
            GameManager.instance.RequestRespawn(this.gameObject, respawnTime);
        }

        spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
    }
}