using System.Collections;
using System.Collections.Generic;
using GameDevTV.UI;
using UnityEngine;

public class ToolbarAudioSelector : MonoBehaviour
{
    [SerializeField] AudioSource openSound = null;
    [SerializeField] AudioSource closeSound = null;

    [SerializeField] GameObject uiContainer = null;

    public void OpensMenuAndPlaysSound()
    {
        if (!uiContainer.GetComponent<ShowHideUI>().isOpened)
        {
            openSound.Play();
        }
        else if (uiContainer.GetComponent<ShowHideUI>().isOpened)
        {
            closeSound.Play();
        }
    }
}
