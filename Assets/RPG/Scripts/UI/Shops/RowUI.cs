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
    public class RowUI : MonoBehaviour, IItemHolder
    {
        [SerializeField] TextMeshProUGUI nameField;

        [SerializeField] Image iconField;



        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI priceField;
        [SerializeField] TextMeshProUGUI quantityField;

        Shop currentShop = null;
        ShopItem item = null;

        //We need to use a serialized field to get hold of the TextMeshPro component of the quantity 
        //field to populate it.
        public void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            this.item = item;
            iconField.sprite = item.GetIcon();
            nameField.text = item.GetName();
            availabilityField.text = $"{item.GetAvailability()}"; //item.GetAvailability();
            priceField.text =  $"{item.GetPrice()}";
            quantityField.text = $"{item.GetQuantityInTransaction()}";  //we need to do this weird shit around the integer to make it readable 
                                                                        //by .text components. remember typing this when making different UI. 
        }

        public void Add()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);
        }

        public InventoryItem GetItem()
        {
            return item.GetInventoryItem();
        }
        public ShopItem GetShopItem()
        {
            return item;
        }
    }
}
