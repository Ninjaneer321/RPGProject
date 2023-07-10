using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI purseText;
        Purse playerPurse;
        void Start()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();

            if (playerPurse != null)
            {
                playerPurse.onChange += RefreshUI;
            }

            RefreshUI();
        }

        private void RefreshUI()
        {
            purseText.text = playerPurse.GetBalance().ToString();
        }

    }
}
