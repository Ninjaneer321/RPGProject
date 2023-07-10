using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class TargetDisplay : MonoBehaviour
    {
        ThirdPersonController thirdPersonController;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Awake()
        {
            thirdPersonController = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (thirdPersonController.GetComponent<Fighter>().GetTarget() != null)
            {
                GetComponent<Text>().enabled = true;
                GetComponent<Text>().text = string.Format("{0:0}", thirdPersonController.GetComponent<Fighter>().GetTarget().name);
            }
            else
            {
                GetComponent<Text>().enabled = false;
                return;
            }


        }
    }
}


