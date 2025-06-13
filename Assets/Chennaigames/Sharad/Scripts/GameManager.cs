using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float entryWalkDuration = 1.5f; 
    public static GameManager Instance;
    public CanvasGroup transitionCanvasGroup; // Drag your TransitionCanvas here
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNextLevel(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Fade to black
        yield return Fade(1f); // Fade In (to black)

        // 2. Load the new scene and unload the old one
        Scene oldScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(oldScene);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        // 3. Move the player to the new start position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject startPos = GameObject.Find("PlayerStartPosition");
        if (player != null && startPos != null)
        {
            // 4. Move the player
            player.transform.position = startPos.transform.position;

            // 5. Start the player's walk-in sequence
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.StartLevelEntrySequence(entryWalkDuration);
            }   
        }

        // 6. Fade from black
        yield return Fade(0f); // Fade Out (to transparency)
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transitionCanvasGroup.alpha = targetAlpha;
    }
}