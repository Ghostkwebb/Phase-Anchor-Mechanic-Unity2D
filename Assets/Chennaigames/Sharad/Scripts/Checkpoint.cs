using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject checkpointEffectPrefab; // Slot for our particle effect

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (checkpointEffectPrefab != null)
            {
                Instantiate(checkpointEffectPrefab, transform.position, Quaternion.identity);
            }

            other.GetComponent<PlayerController>().SetRespawnPoint(transform.position);

            gameObject.SetActive(false);
        }
    }
}