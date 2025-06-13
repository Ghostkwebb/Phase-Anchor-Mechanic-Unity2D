using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialTextController : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public float displayDuration = 2.0f;

    private TextMeshProUGUI textElement;
    private CanvasGroup canvasGroup;
    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        textElement = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; // Ensure it's hidden on start
    }

    public void ShowText(string message)
    {
        // If another text is already fading, stop it and start the new one
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeInAndOut(message));
    }

    private IEnumerator FadeInAndOut(string message)
    {
        textElement.text = message;

        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}