using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vizuály Nepriateľov")]
    public Sprite medusaSprite;    // Tu vlož obrázok medúzy
    public Sprite orcSprite;       // Tu vlož obrázok orka
    public RuntimeAnimatorController medusaAnimator; // Tu vlož Animator pre medúzu

    [Header("Nastavenia")]
    public float maxHealth = 150f;
    private float currentHealth;
    public float respawnTime = 0.3f;
    public int coinReward = 15;

    private float baseMaxHealth;
    private int baseCoinReward;
    private Vector3 originalScale;

    [Header("UI a Efekty")]
    public Slider healthSlider;
    public GameObject damageTextPrefab;
    public Transform canvasTransform;
    public Color damageColor = Color.red;
    public GameObject damageVFXPrefab;
    public GameObject critDamageVFXPrefab;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Animator anim;
    private bool isDead = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        baseMaxHealth = maxHealth;
        baseCoinReward = coinReward;
        originalScale = transform.localScale;
    }

    void Start()
    {
        ResetEnemy();
    }

    void OnMouseDown()
    {
        if (isDead) return;

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
        StartCoroutine(FlashEffect());
        if (currentHealth <= 0) Die();
    }

    public void ResetEnemy()
    {
        isDead = false;

        if (GameManager.instance != null)
        {
            // Logika striedania (0 = Medúza, 1 = Ork)
            if (GameManager.instance.currentEnemyType == 1)
            {
                spriteRenderer.sprite = orcSprite;
                if (anim != null) anim.enabled = false; // Ork nemá animáciu, tak ju vypneme
            }
            else
            {
                spriteRenderer.sprite = medusaSprite;
                if (anim != null)
                {
                    anim.enabled = true;
                    anim.runtimeAnimatorController = medusaAnimator;
                    anim.Rebind(); // Toto reštartuje animáciu, aby nezamrzla
                }
            }

            // Nastavenie štatistík (Boss vs Normal)
            if (GameManager.instance.nextIsBoss)
            {
                currentHealth = maxHealth * 5f;
                transform.localScale = originalScale * 1.3f;
                spriteRenderer.color = new Color(1f, 0.6f, 0.6f);
            }
            else
            {
                currentHealth = maxHealth;
                transform.localScale = originalScale;
                spriteRenderer.color = Color.white;
            }
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = currentHealth;
            healthSlider.value = currentHealth;
            healthSlider.gameObject.SetActive(true);
        }

        spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.AddKill(coinReward);
            GameManager.instance.RequestRespawn(this.gameObject, respawnTime);
        }

        spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);
    }

    // --- Pomocné funkcie pre vizuál ---
    IEnumerator FlashEffect()
    {
        Color oldColor = spriteRenderer.color;
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = oldColor;
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
        tm.text = "-" + GameManager.instance.FormatNumbers(amount) + (isCrit ? "!" : "");
        if (isCrit) tm.color = Color.yellow;
        Destroy(textObj, 1f);
    }
}