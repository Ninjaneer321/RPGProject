using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAudioFinishAndTurnVolumeDown : MonoBehaviour
{

    [SerializeField] AudioSource audioSource = null;
    [SerializeField] float newVolume = 25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator DetectAudioPlaying()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.volume = newVolume;
    }

    public void DetectAudioPlayingCoroutine()
    {
        StartCoroutine(DetectAudioPlaying());
    }
}
