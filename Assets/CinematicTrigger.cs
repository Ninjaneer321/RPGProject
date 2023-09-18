using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using GameDevTV.Saving;

public class CinematicTrigger : MonoBehaviour, ISaveable
{
    public bool alreadyTriggered = false;

    public object CaptureState()
    {
        return alreadyTriggered;
    }

    public void RestoreState(object state)
    {
        alreadyTriggered = (bool)state;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered && other.gameObject.tag == "Player")
        {
            alreadyTriggered = true;
            other.GetComponent<Animator>().SetTrigger("wakeUp");
            StartCoroutine(IntroFadeRoutine());
            GetComponent<PlayableDirector>().Play();
        }
    }

    private IEnumerator IntroFadeRoutine()
    {
        Fader fader = FindObjectOfType<Fader>();
        yield return fader.FadeOut(.01f);
        yield return new WaitForSeconds(2f);
        yield return fader.FadeIn(10f);
    }
}
