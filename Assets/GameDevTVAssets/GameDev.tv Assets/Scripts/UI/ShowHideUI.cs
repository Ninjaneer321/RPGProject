using System.Collections;
using System.Collections.Generic;
using GameDevTV.UI.Inventories;
using UnityEngine;

namespace GameDevTV.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode toggleKey = KeyCode.Escape;
        [SerializeField] GameObject uiContainer = null;
        [SerializeField] GameObject otherInventoryContainer = null;
        [SerializeField] InventoryUI otherInventoryUI = null;
        public bool isOpened = false;

        // Start is called before the first frame update
        void Start()
        {
            uiContainer.SetActive(false);

            if (otherInventoryContainer && otherInventoryUI != null)
            {
                otherInventoryContainer.SetActive(false);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                Toggle();
                if (otherInventoryContainer && otherInventoryUI != null)
                {
                    otherInventoryContainer.SetActive(false);
                }
            }
        }

        public void ShowOtherInventory(GameObject go)
        {
            GameObject player = GameObject.FindWithTag("Player");

            if (otherInventoryContainer && otherInventoryUI != null)
                {
                    isOpened = true;
                    uiContainer.SetActive(true);
                    otherInventoryContainer.SetActive(true);
                    otherInventoryUI.Setup(go);

                    player.GetComponent<InputManager>().lootAudioOpen.Play();
                    player.GetComponent<Animator>().SetBool("isLooting", true);
                    player.GetComponent<Animator>().SetTrigger("lootAction");

                }

        }

        public void Toggle()
        {
            if (isOpened)
            {
                isOpened = false;
            }
            else
            {
                isOpened = true;
            }
            uiContainer.SetActive(!uiContainer.activeSelf);
        }
    }
}