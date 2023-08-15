using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class AbilityShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] Transform listRoot;
        [SerializeField] AbilityRowUI rowPrefab;
        [SerializeField] TextMeshProUGUI basketTotal;
        [SerializeField] Button confirmButton;
        [SerializeField] Button switchButton;

        Shopper shopper = null;
        AbilityShop currentShop = null;

        Color originalTotalTextColor;
        // Start is called before the first frame update
        void Start()
        {
            originalTotalTextColor = basketTotal.color;

            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (shopper == null) return;

            shopper.activeShopChange += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }

        private void ShopChanged()
        {
            if (currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }

            currentShop = shopper.GetActiveAbilityShop();
            gameObject.SetActive(currentShop != null); //currentShop not equalling null is the condition for this object to be active

            foreach (FilterButtonUI buttons in GetComponentsInChildren<FilterButtonUI>())
            {
                buttons.SetAbilityShop(currentShop);
            }
            if (currentShop == null) return;
            shopName.text = currentShop.GetShopName();

            currentShop.onChange += RefreshUI;

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (ShopAbility ability in currentShop.GetFilteredAbilities())
            {
                AbilityRowUI row = Instantiate(rowPrefab, listRoot);
                row.Setup(currentShop, ability);
            }

            basketTotal.text = "Total: " + currentShop.TransactionTotal();
            basketTotal.color = currentShop.HasSufficientFunds() ? originalTotalTextColor : Color.red;

            confirmButton.interactable = currentShop.CanTransact();
            TextMeshProUGUI switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (currentShop.IsBuyingMode())
            {
                switchText.text = "Switch to Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch to Buying";
                confirmText.text = "Sell";
            }
            foreach (FilterButtonUI buttons in GetComponentsInChildren<FilterButtonUI>())
            {
                buttons.RefreshAbilityUI();
            }
        }

        public void CloseAbilityShop()
        {
            shopper.SetActiveShop((AbilityShop)null);
        }

        public void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode()); //inverts the mode when selecting (Buying -> Selling; Selling -> Buying)
        }

        //REENABLE THIS SCRIPT WHEN ABILITYROWUI IS FILLED OUT

        public AbilityItem GetAbility()
        {
            return rowPrefab.GetAbility();
        }
    }
}

