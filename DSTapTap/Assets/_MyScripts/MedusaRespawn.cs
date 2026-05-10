using UnityEngine;
using System.Collections;

public class MedusaRespawn : MonoBehaviour
{
    public float respawnTime = 1f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogError("Kámo, na tomto objekte nie je SpriteRenderer!");
    }

    void Update()
    {
        // Testovacie zabitie klávesou K
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Medúza zomrela, žiadam GameManager o respawn...");
        sr.enabled = false;

        // Namiesto vlastnej Coroutine zavoláme tú v GameManageri
        if (GameManager.instance != null)
        {
            GameManager.instance.RequestRespawn(this.gameObject, respawnTime);
        }
    }

    // Túto funkciu zavolá GameManager po uplynutí času
    public void ResetMedusa()
    {
        sr.enabled = true;
        Debug.Log("Medúza oživená cez GameManager!");
    }
}