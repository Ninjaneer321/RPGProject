using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelperUIBorderAnimation : MonoBehaviour
{
    [SerializeField] CanvasGroup helperUICanvasGroup = null;
    Coroutine currentActiveFade = null;

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        StartCoroutine(HelperUIBorderCoroutine());
    }

    private IEnumerator HelperUIBorderCoroutine()
    {
        FadeOutImmediate();
        yield return new WaitForSeconds(.2f);
        yield return FadeIn(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeOut(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeIn(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeOut(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeIn(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeOut(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeIn(.5f);
        yield return new WaitForSeconds(.2f);
        yield return FadeOut(.5f);
        yield break;
    }
    public void FadeOutImmediate()
    {
        helperUICanvasGroup.alpha = 1;
    }

    public void FadeInImmediate()
    {
        helperUICanvasGroup.alpha = 0;
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
        while (!Mathf.Approximately(helperUICanvasGroup.alpha, target))
        {
            helperUICanvasGroup.alpha = Mathf.MoveTowards(helperUICanvasGroup.alpha, target, Time.unscaledDeltaTime / time);
            yield return null;
        }
    }
}
