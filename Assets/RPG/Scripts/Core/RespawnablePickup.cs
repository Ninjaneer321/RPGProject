using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;

namespace RPG.Core
{
    public class RespawnablePickup : MonoBehaviour
    {
        [SerializeField] float respawnTime = 5;


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }
        private void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
        public void Pickup()
        {
            Debug.Log("RespawnablePickup Pickup()");
            StartCoroutine(HideForSeconds(respawnTime));
        }
    }
}

