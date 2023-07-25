using System.Collections;
using System.Collections.Generic;
using RPG.UI;
using UnityEngine;

public class TownZone : MonoBehaviour
{

    [SerializeField] BoxCollider sphereCollider;
    [SerializeField] string zoneName = null;

    PlayerUICanvas playerUICanvas;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerUICanvas = other.GetComponentInChildren<PlayerUICanvas>();
            playerUICanvas.zoneCanvas.SetActive(true);
            playerUICanvas.zoneTextMeshPro.text = zoneName;
            StartCoroutine(Transition());
        }

    }

    private IEnumerator Transition()
    {
        ZoneTextFader fader = FindObjectOfType<ZoneTextFader>();
        yield return fader.FadeOut(3);
        yield return new WaitForSeconds(2);
        yield return fader.FadeIn(5);
        playerUICanvas = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerUICanvas>();
        playerUICanvas.zoneCanvas.SetActive(false);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        PlayerUICanvas playerUICanvas = other.GetComponentInChildren<PlayerUICanvas>();
    //        ZoneTextFader fader = FindObjectOfType<ZoneTextFader>();
    //        if (fader != null) Debug.Log("Fader found");
    //        playerUICanvas.zoneTextMeshPro.text = "";
    //        fader.FadeInImmediate();
    //        Debug.Log("Exiting town");
    //    }
    //}
}
