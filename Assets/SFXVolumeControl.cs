using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeControl : MonoBehaviour, ISaveable
{
    // Start is called before the first frame update

    [SerializeField] Slider sfxSlider = null;
    [SerializeField] public List<AudioSource> audioSourcesList = new List<AudioSource>();

    //FORCING AN ARTIFICIAL WAIT TIME BECAUSE OF ENEMY SPAWNING AND THEIR AUDIOSOURCES

    [SerializeField] float artificialTimeToWait = 0.25f;
    public List<AudioSource> GetSFXChildren()
    {
        audioSourcesList.Clear();

        GameObject[] sfxParents = GameObject.FindGameObjectsWithTag("SFXSource");

        foreach (GameObject parent in sfxParents)
        {
            Transform parentTransform = parent.transform;
            int childCount = parentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = parentTransform.GetChild(i);
                AudioSource childObject = childTransform.gameObject.GetComponent<AudioSource>();
                if (!childObject.CompareTag("IgnoreSFXVolume"))
                {
                    audioSourcesList.Add(childObject);
                }

            }
        }

        return audioSourcesList;
    }

    public void SetSFXVolume()
    {
        foreach (AudioSource audioSource in audioSourcesList)
        {
            audioSource.volume = sfxSlider.normalizedValue;
        }
    }

    private IEnumerator GetSFXChildrenCoroutine()
    {
        yield return new WaitForSeconds(artificialTimeToWait);
        GetSFXChildren();
    }
    private void Start()
    {
        StartCoroutine(GetSFXChildrenCoroutine());
        SetSFXVolume();
    }

    public object CaptureState()
    {
        return sfxSlider.normalizedValue;
    }

    public void RestoreState(object state)
    {
        sfxSlider.normalizedValue = (float)state;
    }
}