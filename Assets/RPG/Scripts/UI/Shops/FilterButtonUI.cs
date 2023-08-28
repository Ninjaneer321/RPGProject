using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] ItemCategory category = ItemCategory.None;

        Button button;
        Shop currentItemShop;


        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop currentShop)
        {
            this.currentItemShop = currentShop;
        }


        public void RefreshItemUI()
        {
            button.interactable = (currentItemShop.GetFilter() != category);
        }

        private void SelectFilter()
        {
            currentItemShop.SelectFilter(category);
        }
    }
}
