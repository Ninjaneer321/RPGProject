using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Shops
{
    public class ShopItem
    {
        InventoryItem item; //we don't need a field for the name because this comes with the IventoryItem item
        int availability;
        float price;
        int quantityInTransaction;
        

        //We need to add a quantity getter to get hold of it in RowUI.
        public string GetName()
        {
            return item.GetDisplayName();
        }
        public Sprite GetIcon()
        {
            return item.GetIcon();
        }
        
        //public string GetRequiredLevel()
        //{
        //    return item.GetRequiredLevel().ToString();
        //}

        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public float GetPrice()
        {
            return price;
        }

        public InventoryItem GetInventoryItem()
        {
            return item;
        }

        public int GetQuantityInTransaction()
        {
            return quantityInTransaction;
        }

        public int GetAvailability()
        {
            return availability;
        }
    }
}
