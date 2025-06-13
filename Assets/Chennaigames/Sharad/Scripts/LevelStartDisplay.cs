using UnityEngine;
using TMPro;
using System.Collections;

public class LevelStartDisplay : MonoBehaviour
{
    [Header("Text Configuration")]
    public string levelTitle = "Level 1";
    public string levelSubtitle = "Tutorial";

    [Header("Animation Timings")]
    public float fadeInDuration = 1.5f;
    public float displayDuration = 2.0f;
    public float fadeOutDuration = 1.5f;

    private TextMeshProUGUI textElement;
    private CanvasGroup canvasGroup;

    void Start()
    {
        textElement = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Set the text content, using rich text for sizing
        textElement.text = $"{levelTitle}\n<size=70%>{levelSubtitle}</size>";

        // Start the animation routine
        StartCoroutine(DisplayRoutine());
    }

    private IEnumerator DisplayRoutine()
    {
        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = 1 - Mathf.Clamp01(elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        
        Destroy(gameObject);
    }
}