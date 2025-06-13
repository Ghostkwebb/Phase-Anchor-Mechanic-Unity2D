using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public string sceneToLoad; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the GameManager to start the transition
            GameManager.Instance.LoadNextLevel(sceneToLoad);
        }
    }
}