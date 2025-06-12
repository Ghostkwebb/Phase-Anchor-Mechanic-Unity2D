using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configuration")]
    public float lifetime = 5f; // Fallback to destroy if it hits nothing

    [Header("Effects")]
    public GameObject impactEffectPrefab; // The particle effect to spawn

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Schedule destruction as a fallback
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the player completely
        if (other.CompareTag("Player"))
        {
            return;
        }

        // If it hits a wall, stick to it so the player can teleport
        if (other.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            // We DON'T destroy it here
        }
        else // If it hits anything else (like the Ground)
        {
            // Spawn the impact particles
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }
            // And destroy the projectile
            Destroy(gameObject);
        }
    }
}