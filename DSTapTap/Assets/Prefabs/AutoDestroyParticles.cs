using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticles : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // Získame čas, ale pridáme 1 sekundu ako poistku
        float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax + 1.0f;

        // Zničíme objekt
        Destroy(gameObject, totalDuration);
    }
}