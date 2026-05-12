using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vizuály Nepriateľov")]
    public Sprite medusaSprite;
    public Sprite orcSprite;
    public RuntimeAnimatorController medusaAnimator;

    [Header("Nastavenia")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 0.3f;
    public int coinReward = 15;

    [Header("UI a Efekty")]
    public Slider healthSlider;
    public GameObject damageTextPrefab;
    public Transform canvasTransform;
    public Color damageColor = Color.red;
    public GameObject damageVFXPrefab;
    public GameObject critDamageVFXPrefab;

    [Header("Nastavenia Klikania")]
    public float damageCooldown = 0.1f;
    private float nextDamageTime = 0f;

    private float baseMaxHealth;
    private int baseCoinReward;
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Animator anim;
    private Color originalColor;
    private Coroutine bossTimerCoroutine;
    private bool isBoss = false;
    private bool isDead = false;

    // Ochrana proti zaseknutiu farby pri rýchlom klikaní
    private bool isFlashing = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        baseMaxHealth = maxHealth;
        baseCoinReward = coinReward;
        originalScale = transform.localScale;

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Start()
    {
        ResetEnemy();
    }

    void OnMouseDown()
    {
        if (isDead || Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + damageCooldown;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddClick();
            float damageToDeal = GameManager.instance.clickDamage;
            bool isCrit = Random.Range(0f, 100f) <= GameManager.instance.critChance;

            if (isCrit) damageToDeal *= GameManager.instance.critMultiplier;

            GameManager.instance.AddDamageStat(damageToDeal);
            TakeDamage(damageToDeal);
            SpawnVFXAtMouse(isCrit);
            SpawnDamagePopup(damageToDeal, isCrit);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        // Spustenie bliknutia
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0) Die();
    }

    public void ResetEnemy()
    {
        isDead = false;
        isFlashing = false; // Reset blikania pri novom nepriateľovi

        if (bossTimerCoroutine != null) StopCoroutine(bossTimerCoroutine);

        if (GameManager.instance != null)
        {
            // Reset farby na pôvodnú (bielu)
            if (spriteRenderer != null) spriteRenderer.color = originalColor;

            // Logika vizuálu (Medúza vs Ork)
            if (GameManager.instance.currentEnemyType == 1)
            {
                if (spriteRenderer != null) spriteRenderer.sprite = orcSprite;
                if (anim != null) anim.enabled = false;
            }
            else
            {
                if (spriteRenderer != null) spriteRenderer.sprite = medusaSprite;
                if (anim != null)
                {
                    anim.enabled = true;
                    anim.runtimeAnimatorController = medusaAnimator;
                    anim.Rebind();
                }
            }

            // Boss nastavenia
            if (GameManager.instance.nextIsBoss)
            {
                isBoss = true;
                currentHealth = baseMaxHealth * 5f;
                coinReward = baseCoinReward * 10;
                transform.localScale = originalScale * 1.3f;
                if (spriteRenderer != null) spriteRenderer.color = new Color(1f, 0.6f, 0.6f);
                bossTimerCoroutine = StartCoroutine(BossTimer(GameManager.instance.bossTimeLimit));
            }
            else
            {
                isBoss = false;
                currentHealth = baseMaxHealth;
                coinReward = baseCoinReward;
                transform.localScale = originalScale;
                // Tu už neprepíšeme farbu na bielu, ak sme ju nastavili v Awake
            }
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = currentHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(true);
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
    }

    private IEnumerator BossTimer(float timeLimit)
    {
        float timeLeft = timeLimit;
        while (timeLeft > 0)
        {
            if (GameManager.instance != null && GameManager.instance.bossProgressText != null)
            {
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

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
    }

    IEnumerator FlashEffect()
    {
        // Ak už blikáme, nepúšťaj ďalšie blikanie (zamedzí zaseknutiu farby)
        if (spriteRenderer == null || isFlashing) yield break;

        isFlashing = true;
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(0.1f);

        // Vráti farbu na pôvodnú uloženú v Awake
        spriteRenderer.color = isBoss ? new Color(1f, 0.6f, 0.6f) : originalColor;
        isFlashing = false;
    }

    void SpawnVFXAtMouse(bool isCrit)
    {
        GameObject prefab = isCrit ? critDamageVFXPrefab : damageVFXPrefab;
        if (prefab == null) return;
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = -1f;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    void SpawnDamagePopup(float amount, bool isCrit)
    {
        if (damageTextPrefab == null || canvasTransform == null) return;
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(transform.position);
        GameObject textObj = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvasTransform);
        var tm = textObj.GetComponent<TMPro.TextMeshProUGUI>();
        if (tm != null)
        {
            tm.text = "-" + GameManager.instance.FormatNumbers(amount) + (isCrit ? "!" : "");
            if (isCrit) tm.color = Color.yellow;
        }
        Destroy(textObj, 1f);
    }
}