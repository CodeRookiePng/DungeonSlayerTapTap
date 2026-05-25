using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vizuály Nepriateľov")]
    public Sprite medusaSprite;
    public RuntimeAnimatorController medusaAnimator;

    [Header("Skriptová Animácia Orka")]
    public List<Sprite> orcAnimationFrames;
    public float animationSpeed = 0.1f;

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
    private float bossTimeLeft; // Pamätá si zostávajúci čas bosa pri prepínaní obrazoviek

    private Coroutine orcAnimationCoroutine;
    private bool isBoss = false;
    private bool isDead = false;
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

    void OnEnable()
    {
        // Reštart animácie Orka po návrate
        if (GameManager.instance != null && GameManager.instance.currentEnemyType == 1 && !isDead)
        {
            if (orcAnimationCoroutine != null) StopCoroutine(orcAnimationCoroutine);
            orcAnimationCoroutine = StartCoroutine(AnimateOrc());
        }

        // OPRAVA TIMERA: Ak bol nepriateľ boss a ešte nezomrel, po návrate z upgradov obnovíme timer tam, kde skončil
        if (isBoss && !isDead && bossTimeLeft > 0)
        {
            if (bossTimerCoroutine != null) StopCoroutine(bossTimerCoroutine);
            bossTimerCoroutine = StartCoroutine(BossTimer(bossTimeLeft));
        }
    }

    void OnDisable()
    {
        // Bezpečne stopneme animáciu Orka
        if (orcAnimationCoroutine != null)
        {
            StopCoroutine(orcAnimationCoroutine);
            orcAnimationCoroutine = null;
        }

        // OPRAVA TIMERA: Stopneme odpočítavanie bosa, ale čas (sekundy) necháme uložený v bossTimeLeft
        if (bossTimerCoroutine != null)
        {
            StopCoroutine(bossTimerCoroutine);
            bossTimerCoroutine = null;
        }
    }

    void OnMouseDown()
    {
        if (isDead || Time.time < nextDamageTime) return;
        nextDamageTime = Time.time + damageCooldown;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddClick();

            // ZMENA: Výpočet finálneho DMG (vrátane itemov a kritického zásahu) rieši priamo GameManager
            float damageToDeal = GameManager.instance.GetFinalClickDamage();

            // Zistíme spätne, či to bol crit, porovnaním vypočítaného damage oproti bežnému (na účely vizuálnych efektov)
            // Bežný damage je: clickDamage * (1f + activeDamageBoost)
            float normalDamageWithItems = GameManager.instance.clickDamage * (1f + GameManager.instance.activeDamageBoost);
            bool isCrit = damageToDeal > normalDamageWithItems;

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

        StartCoroutine(FlashEffect());

        if (currentHealth <= 0) Die();
    }

    public void ResetEnemy()
    {
        isDead = false;
        isFlashing = false;

        if (bossTimerCoroutine != null) StopCoroutine(bossTimerCoroutine);
        if (orcAnimationCoroutine != null) StopCoroutine(orcAnimationCoroutine);

        if (GameManager.instance != null)
        {
            if (spriteRenderer != null) spriteRenderer.color = originalColor;

            if (GameManager.instance.currentEnemyType == 1)
            {
                if (anim != null) anim.enabled = false;
                orcAnimationCoroutine = StartCoroutine(AnimateOrc());
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

                // ZMENA: Pri úplne novom bossovi vytiahneme čas upravený o sekundy z itemov
                bossTimeLeft = GameManager.instance.GetFinalBossTime();
                bossTimerCoroutine = StartCoroutine(BossTimer(bossTimeLeft));
            }
            else
            {
                isBoss = false;
                currentHealth = baseMaxHealth;
                coinReward = baseCoinReward;
                transform.localScale = originalScale;
                bossTimeLeft = 0;
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

    private IEnumerator AnimateOrc()
    {
        if (orcAnimationFrames == null || orcAnimationFrames.Count == 0) yield break;

        int currentFrame = 0;
        while (true)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = orcAnimationFrames[currentFrame];
            }

            currentFrame = (currentFrame + 1) % orcAnimationFrames.Count;
            yield return new WaitForSeconds(animationSpeed);
        }
    }

    private IEnumerator BossTimer(float startTime)
    {
        bossTimeLeft = startTime;
        while (bossTimeLeft > 0)
        {
            if (GameManager.instance != null && GameManager.instance.bossProgressText != null)
            {
                GameManager.instance.bossProgressText.text = "TIME: " + bossTimeLeft.ToString("F1") + "s";
            }
            yield return null;
            bossTimeLeft -= Time.deltaTime;
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
        if (orcAnimationCoroutine != null) StopCoroutine(orcAnimationCoroutine);
        bossTimeLeft = 0;

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
        if (spriteRenderer == null || isFlashing) yield break;

        isFlashing = true;
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(0.1f);

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