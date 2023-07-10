using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, /*IRaycastable,*/ ISaveable
    {
        [SerializeField] string shopName;
        [Range(0, 100)]
        [SerializeField] float sellingPercentage = 80f;

        // Stock Config:
        // Item:
        // InventoryItem
        // Initial Stock
        // buyingDiscountPercentage

        //[SerializeField] StockItemConfig[] stockConfig;

        [SerializeField] List<StockItemConfig> stockConfig = new List<StockItemConfig>();
        List<InventoryItem> sellingList = new List<InventoryItem>();

        [System.Serializable]
        public class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)]
            public float buyingDiscountPercentage;

            public StockItemConfig(InventoryItem item, int initialStock)
            {
                this.item = item;
                this.initialStock = initialStock;
                buyingDiscountPercentage = 0;
            }
        }
        public event Action onChange;

        private void Awake()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                stock[config.item] = config.initialStock;
            }
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stock = new Dictionary<InventoryItem, int>();
        private Shopper currentShopper;
        bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.None;

        public string GetShopName()
        {
            return shopName;
        }

        public void SetShopper(Shopper shopper)
        {
            this.currentShopper = shopper;
        }

        public void OpenShop()
        {
            Debug.Log("OpenShop!");
            GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>().SetActiveShop(this);
            //playerManager.GetComponent<Shopper>().SetActiveShop(this);
        }
            
       public IEnumerable<ShopItem> GetFilteredItems()          
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if (filter == ItemCategory.None || item.GetCategory() == filter)
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            sellingList.Clear();

            if (isBuyingMode) //This bit of code populates the store list with items to buy
            {
                foreach (StockItemConfig config in stockConfig)
                {
                    float price = GetPrice(config);
                    int quantityInTransaction = 0;
                    transaction.TryGetValue(config.item, out quantityInTransaction);
                    //We start off with a default value of 0. If we have the item in the transaction, it will populate and override the 0. If not, it will ignore it.

                    int availability = GetAvailability(config.item);
                    yield return new ShopItem(config.item, availability, price, quantityInTransaction);
                }
            }
            else //This bit of code populates the store list with items to sell (coming from our Inventory)
            {
                Inventory shopperInventory = currentShopper.GetComponent<Inventory>();      //grabbing Inventory component
                for (int i = 0; i < shopperInventory.GetSize(); i++)                        //Basic for loop over the size of the player Inventory
                {
                    InventoryItem item = shopperInventory.GetItemInSlot(i);                 //creates an Inventory object for the items in the inventory slots
                    if (item != null)                                                       //if the object exists in the context 
                    {
                        int quantityInTransaction = 0;                                      //local int variable for quantity (necessary)
                        transaction.TryGetValue(item, out quantityInTransaction);           //If we have the item in the transaction, it will populate and override the 0. If not, it will ignore it.
                        float price = 0;

                        if (stock.ContainsKey(item))                                        //If the item already exists in the stock, it uses the
                        {                                                                   //regular discount for this item. Otherwise the selling price
                            foreach(StockItemConfig config in stockConfig)                  //amounts to 80% of the original price.
                            {
                                if (config.item == item)
                                {
                                    price = GetPrice(config);
                                }
                            }
                        }
                        else
                        {
                            price = item.GetPrice() * 0.8f;
                        }
                        if (!sellingList.Contains(item))
                        {
                            sellingList.Add(item);
                            yield return new ShopItem(item, GetAvailability(item), price, quantityInTransaction);
                        }
                    }
                }
            }
        }

      
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            //Transfer to or from the inventory

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }

            if (onChange != null)
            {
                onChange();
            }
            //Removal from transaction
            //check if successful
            //deduct from transaction (we have this method)
            //any errors?
            //Debiting or Crediting of funds
        }

        public void SelectFilter(ItemCategory category)
        {
            filter = category;
            if (onChange != null)
            {
                onChange();
            }
        }

        public ItemCategory GetFilter()
        {
            return filter;
        }
        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;
            if (onChange != null)
            {
                onChange();
            }
        }
        public bool IsBuyingMode()
        {
            return isBuyingMode;
        }
        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }

        private bool HasInventorySpace()
        {
            if (!isBuyingMode) return true; //if we are selling, we don't need sufficient inventory space therefore this will always be true

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) return true; //if we are selling, we don't need sufficient funds therefore this will always return true

            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperPurse == null) return false;

            return shopperPurse.GetBalance() >= TransactionTotal();

        }

        private bool IsTransactionEmpty()
        {
            return transaction.Count == 0; //this returns true if the transaction count is zero
        }

        public float TransactionTotal()
        {
            //Iterate over all of the shop items
            //Keep a running total
            //Be sure to multiply by the quantity of the items in the basket

            float total = 0;

            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
                //total = total + (item.GetPrice() * item.GetQuantityInTransaction());
            }

            return total;
            
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))         //We need to give this dictionary a default starting point. We check to see if the 
            {                                           //dictionary contains the key "item". If it doesn't, we want to give it a key item 
                transaction[item] = 0;                  //and give it a default value. = 0 is a classic default value and makes the next code work.
            }

            int availability = GetAvailability(item);
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;              //transaction[item] allows us access to a specific item in the dictionary,
                                                            //and in turn gives us a quantity to deal with. We add to the quantity that was 
                                                            //passed into the method.   
            }
            if (transaction[item] <= 0)
            {                                           //If the quantity of the transaction item goes below 0 in the transaction, 
                transaction.Remove(item);               //we want to remove it from the dictionary because the lowest value is 0.
            }

            if (onChange != null)
            {
                onChange();
            }
        }
        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }
        //public bool HandleRaycast(PlayerManager playerManager)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        playerManager.GetComponent<Shopper>().SetActiveShop(this);
        //    }
        //    return true;
        //}
        private int GetAvailability(InventoryItem item)
        {
            if (isBuyingMode)
            {
                return stock[item];
            }

            return CountItemsInInventory(item);
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            //Get Hold of Shopper Inventory, do something different if you cant.
            //Loop over GetSize, get the numbers of the slots
            //Check if item matches with GetItemInSlot()
            //total up the amounts with GetNumberInSlot();

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return 0;
            int total = 0;
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    total += shopperInventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        private float GetPrice(StockItemConfig config)
        {
            if (isBuyingMode)
            {
                return config.item.GetPrice() * (1 - config.buyingDiscountPercentage / 100);
            }

            return config.item.GetPrice() * (sellingPercentage / 100); //if we are in selling mode (!isBuyingMode)
        }
        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;
            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);

            if (stock.ContainsKey(item))
            {
                stock[item]++;
            }
            else
            {
                stockConfig.Add(new StockItemConfig(item, 1));
                stock[item] = 1;
            }
            shopperPurse.UpdateBalance(price);
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBalance() < price) return;

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                stock[item]--;
                shopperPurse.UpdateBalance(-price); //you can use - as a minus sign in math!
            }
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            //Iterate through inventory
            //Find item that matches
            //return appropriate slot number, if cant find any return -1 (because -1 as a slot in an array doesnt exist)

            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return -1;
            
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach (var pair in stock)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;

            }

            return saveObject;

        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            stock.Clear();

            foreach (var pair in saveObject)
            {
                stock[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }
}
