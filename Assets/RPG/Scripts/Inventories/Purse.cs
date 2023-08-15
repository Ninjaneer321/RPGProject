using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore
    {
        [SerializeField] float startingBalance = 1000f;

        float balance = 0;

        public event Action onChange;
        private void Awake()
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            if (onChange != null)
            {
                onChange();
            }

        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;

        }

        public int AddInventoryItems(InventoryItem item, int number)
        {

                if (item is CurrencyItem)
                {
                string newItemString = "<br>Item received: " + item.GetDisplayName() + ". x:" + number.ToString() + ".";
                ChatBox chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();
                chatBox.UpdateText(newItemString);
                UpdateBalance(item.GetPrice() * number);
                    return number;
                }
                return 0;
        }

        public int AddAbilityItems(AbilityItem ability, int number)
        {
            Debug.Log("Purchased Ability Items");
            throw new NotImplementedException();
        }
    }
}
