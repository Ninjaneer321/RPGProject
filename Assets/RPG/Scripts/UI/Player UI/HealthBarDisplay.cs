using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;

public class HealthBarDisplay : MonoBehaviour
{
    [SerializeField] Health healthComponent = null;
    [SerializeField] RectTransform foreground = null;
    [SerializeField] Canvas rootCanvas = null;
    [SerializeField] Fighter fighterComponent = null;



    private void Awake()
    {

    }

    void Update()
    {

        if (Mathf.Approximately(healthComponent.GetFraction(), 0) 
            || Mathf.Approximately(healthComponent.GetFraction(), 1)
            || fighterComponent.target == null)
        {
            rootCanvas.enabled = false;
            return;
        }

        rootCanvas.enabled = true;
        foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
    }
}
