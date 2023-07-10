using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Dialogue;
using RPG.Stats;
using TMPro;
using UnityEngine;

public class NamePlateDisplay : MonoBehaviour
{
    [SerializeField] Health healthComponent = null;
    [SerializeField] Canvas rootCanvas = null;
    [SerializeField] Fighter fighterComponent = null;
    [SerializeField] TextMeshProUGUI namePlateText = null;

    // Start is called before the first frame update
    private void Awake()
    {
        if (this.transform.parent.tag == "NPC")
        {
            namePlateText.text = GetComponentInParent<AIConversant>().conversantName;
        }

    }
        // Update is called once per frame
        void Update()
    {
        //if (Mathf.Approximately(healthComponent.GetFraction(), 0)
        //    || Mathf.Approximately(healthComponent.GetFraction(), 1)
        //    || fighterComponent.target == null)
        //{
        //    rootCanvas.enabled = false;
        //    return;
        //}

        rootCanvas.enabled = true;
    }
}
