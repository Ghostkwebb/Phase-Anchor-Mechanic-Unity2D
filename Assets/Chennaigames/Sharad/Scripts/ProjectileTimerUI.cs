using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProjectileTimerUI : MonoBehaviour
{
    private Image timerImage;
    private CanvasGroup canvasGroup;
    private Coroutine currentTimerCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        timerImage = GetComponentInChildren<Image>();
        canvasGroup.alpha = 0; // Start hidden
    }

    public void StartTimer(float duration)
    {
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
        }
        currentTimerCoroutine = StartCoroutine(DepleteRoutine(duration));
    }

    public void Hide()
    {
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
        }
        canvasGroup.alpha = 0;
    }

    public bool IsVisible()
    {
        return canvasGroup.alpha > 0;
    }

    private IEnumerator DepleteRoutine(float duration)
    {
        canvasGroup.alpha = 1;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            timerImage.fillAmount = 1 - (elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0; // Hide when finished
    }
}