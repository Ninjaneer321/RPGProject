using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeControl : MonoBehaviour, ISaveable
{
    // Start is called before the first frame update

    [SerializeField] Slider musicSlider = null;
    [SerializeField] AudioSource musicVolumeSource = null;

    public void SetMusicVolume()
    {
        musicVolumeSource.volume = musicSlider.normalizedValue;
    }

    private void Awake()
    {
        SetMusicVolume();
    }

    public object CaptureState()
    {
        return musicSlider.normalizedValue;
    }

    public void RestoreState(object state)
    {
        musicSlider.normalizedValue = (float)state;
    }
}
