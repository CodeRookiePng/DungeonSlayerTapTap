using UnityEngine;
using UnityEngine.UI;

public partial class EnemyHealth : MonoBehaviour
{
    [Header("Nastavenia Nepriateľa")]
    public float maxHealth = 150f;
    private float currentHealth;

    [Header("UI Prepojenie")]
    public Slider healthSlider;
    public GameObject damageTextPrefab;
    public Transform canvasTransform;

    [Header("Efekty")]
    public Color damageColor = Color.red;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }
    }

    void OnMouseDown()
    {
        TakeDamage(10f);
    }

    void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthSlider != null) healthSlider.value = currentHealth;

        if (damageTextPrefab != null && canvasTransform != null)
        {
            GameObject textObj = Instantiate(damageTextPrefab, Input.mousePosition, Quaternion.identity, canvasTransform);

            // Fix: Ak by to hádzalo chybu s LastSibling, pridaj to sem
            textObj.transform.SetAsLastSibling();

            textObj.GetComponent<TMPro.TextMeshProUGUI>().text = "-" + amount.ToString();
            Destroy(textObj, 1f);
        }

        StopAllCoroutines();
        StartCoroutine(FlashEffect());
        if (currentHealth <= 0) Die();
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