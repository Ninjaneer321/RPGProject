using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTextFader : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    Coroutine currentActiveFade = null;


    private void Awake()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();

    }
    public void FadeOutImmediate()
    {
        canvasGroup.alpha = 1;
    }

    public void FadeInImmediate()
    {
        canvasGroup.alpha = 0;
    }
    public Coroutine FadeOut(float time)
    {
        return Fade(1, time);
    }

    public Coroutine FadeIn(float time)
    {
        return Fade(0, time);
    }

    public Coroutine Fade(float target, float time)
    {
        if (currentActiveFade != null)
        {
            StopCoroutine(currentActiveFade);
        }
        currentActiveFade = StartCoroutine(FadeRoutine(target, time));
        return currentActiveFade;
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.unscaledDeltaTime / time);
            yield return null;
        }
    }
}