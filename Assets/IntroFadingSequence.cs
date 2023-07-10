using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroFadingSequence : MonoBehaviour
{

    public void FadingSequenceIntro()
    {
        Debug.Log("FadingSequence");

        Fader fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        fader.FadeIn(5f);
    }
}
