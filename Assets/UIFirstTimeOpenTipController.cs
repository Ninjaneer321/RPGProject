using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using GameDevTV.UI;
using UnityEngine;

public class UIFirstTimeOpenTipController : MonoBehaviour, ISaveable
{
    [SerializeField] ShowHideUI showHideUI = null;
    [SerializeField] GameObject helperUIPrefab = null;
    public bool hasBeenOpenedOnce = false;
    public object CaptureState()
    {
        return hasBeenOpenedOnce;
    }

    public void RestoreState(object state)
    {
        hasBeenOpenedOnce = (bool)state;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasBeenOpenedOnce && showHideUI.isOpened)
        {
            helperUIPrefab.SetActive(true);
            hasBeenOpenedOnce = true;
        }
    }

}
