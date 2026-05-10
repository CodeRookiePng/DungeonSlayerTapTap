using UnityEngine;
using System.Collections;

public class MedusaRespawn : MonoBehaviour
{
    public float respawnTime = 2f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogError("Kámo, na tomto objekte nie je SpriteRenderer!");
        Debug.Log("Skript nabehol na: " + gameObject.name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Stlačil si K, skúšam zabiť Medúzu...");
            Die();
        }
    }

    public void Die()
    {
        StartCoroutine(RespawnSequence());
    }

    IEnumerator RespawnSequence()
    {
        sr.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        sr.enabled = true;
        Debug.Log("Medúza oživená!");
    }
}