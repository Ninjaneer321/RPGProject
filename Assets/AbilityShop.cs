using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Control;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
    public class AbilityShop : MonoBehaviour
    {
        [SerializeField] string shopName;
        [Range(0, 100)]
        [SerializeField] float sellingPercentage = 80f;

        [SerializeField] List<StockItemConfig> stockConfig = new List<StockItemConfig>();
        List<AbilityItem> sellingList = new List<AbilityItem>();

        [System.Serializable]
        public class StockItemConfig
        {
            public AbilityItem ability;
            public int initialStock;
            [Range(0, 100)]
            public float buyingDiscountPercentage;

            public StockItemConfig(AbilityItem item, int initialStock)
            {
                this.ability = item;
                this.initialStock = initialStock;
                buyingDiscountPercentage = 0;
            }
        }
        public event Action onChange;

        private void Awake()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                stock[config.ability] = config.initialStock;
            }
        }
        Dictionary<AbilityItem, int> transaction = new Dictionary<AbilityItem, int>();
        Dictionary<AbilityItem, int> stock = new Dictionary<AbilityItem, int>();
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
        public IEnumerable<ShopAbility> GetFilteredAbilities()
        {
            foreach (ShopAbility shopAbility in GetAllAbilities())
            {
                AbilityItem item = shopAbility.GetAbilityItem();
                if (filter == ItemCategory.None || item.GetCategory() == filter)
                {
                    yield return shopAbility;
                }
            }
        }
        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }
        private bool IsTransactionEmpty()
        {
            return transaction.Count == 0; //this returns true if the transaction count is zero
        }
        private bool HasInventorySpace()
        {
            if (!isBuyingMode) return true; //if we are selling, we don't need sufficient inventory space therefore this will always be true

            AbilityInventory shopperInventory = currentShopper.GetComponent<AbilityInventory>();
            if (shopperInventory == null) return false;

            List<AbilityItem> flatItems = new List<AbilityItem>();
            foreach (ShopAbility shopAbility in GetAllAbilities())
            {
                AbilityItem item = shopAbility.GetAbilityItem();
                int quantity = shopAbility.GetQuantityInTransaction();
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

        public float TransactionTotal()
        {
            //Iterate over all of the shop items
            //Keep a running total
            //Be sure to multiply by the quantity of the items in the basket

            float total = 0;

            foreach (ShopAbility ability in GetAllAbilities())
            {
                total += ability.GetPrice() * ability.GetQuantityInTransaction();
                //total = total + (item.GetPrice() * item.GetQuantityInTransaction());
            }

            return total;

        }
        private float GetPrice(StockItemConfig config)
        {
            if (isBuyingMode)
            {
                return config.ability.GetPrice() * (1 - config.buyingDiscountPercentage / 100);
            }

            return config.ability.GetPrice() * (sellingPercentage / 100); //if we are in selling mode (!isBuyingMode)
        }

        private int GetAvailability(AbilityItem ability)
        {
            if (isBuyingMode)
            {
                return stock[ability];
            }

            return CountAbilitiesInInventory(ability);
        }

        private int CountAbilitiesInInventory(AbilityItem ability)
        {
            //Get Hold of Shopper Inventory, do something different if you cant.
            //Loop over GetSize, get the numbers of the slots
            //Check if item matches with GetItemInSlot()
            //total up the amounts with GetNumberInSlot();

            AbilityInventory shopperInventory = currentShopper.GetComponent<AbilityInventory>();
            if (shopperInventory == null) return 0;
            int total = 0;
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetAbilityInSlot(i) == ability)
                {
                    total += shopperInventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        public IEnumerable<ShopAbility> GetAllAbilities()
        {
            sellingList.Clear();

            if (isBuyingMode) //This bit of code populates the store list with items to buy
            {
                foreach (StockItemConfig config in stockConfig)
                {
                    float price = GetPrice(config);
                    int quantityInTransaction = 0;
                    transaction.TryGetValue(config.ability, out quantityInTransaction);
                    //We start off with a default value of 0. If we have the item in the transaction, it will populate and override the 0. If not, it will ignore it.

                    int availability = GetAvailability(config.ability);
                    yield return new ShopAbility(config.ability, availability, price, quantityInTransaction);
                }
            }
            else //This bit of code populates the store list with items to sell (coming from our Inventory)
            {

                AbilityInventory shopperInventory = currentShopper.GetComponent<AbilityInventory>();      //grabbing Inventory component : NEED TO GRAB ABILITIES COMPONENT


                for (int i = 0; i < shopperInventory.GetSize(); i++)                        //Basic for loop over the size of the player Inventory
                {
                    AbilityItem ability = shopperInventory.GetAbilityInSlot(i);                 //creates an Inventory object for the items in the inventory slots
                    if (ability != null)                                                       //if the object exists in the context 
                    {
                        int quantityInTransaction = 0;                                      //local int variable for quantity (necessary)
                        transaction.TryGetValue(ability, out quantityInTransaction);           //If we have the item in the transaction, it will populate and override the 0. If not, it will ignore it.
                        float price = 0;

                        if (stock.ContainsKey(ability))                                        //If the item already exists in the stock, it uses the
                        {                                                                   //regular discount for this item. Otherwise the selling price
                            foreach (StockItemConfig config in stockConfig)                  //amounts to 80% of the original price.
                            {
                                if (config.ability == ability)
                                {
                                    price = GetPrice(config);
                                }
                            }
                        }
                        else
                        {
                            price = ability.GetPrice() * 0.8f;
                        }
                        if (!sellingList.Contains(ability))
                        {
                            sellingList.Add(ability);
                            yield return new ShopAbility(ability, GetAvailability(ability), price, quantityInTransaction);
                        }
                    }
                }
            }
        }
        public void ConfirmTransaction()
        {
            AbilityInventory shopperInventory = currentShopper.GetComponent<AbilityInventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            //Transfer to or from the inventory

            foreach (ShopAbility shopAbility in GetAllAbilities())
            {
                AbilityItem item = shopAbility.GetAbilityItem();
                int quantity = shopAbility.GetQuantityInTransaction();
                float price = shopAbility.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        //WE NEED TO EDIT BUTYITEM SO IT BUYS ABILITIES AND PUTS IT IN AN ABILITY SLOT


                        BuyAbility(shopperInventory, shopperPurse, item, price);

                    }
                    else
                    {
                        //SellItem(shopperInventory, shopperPurse, item, price);
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

        private void BuyAbility(AbilityInventory shopperInventory, Purse shopperPurse, AbilityItem ability, float price)
        {
            if (shopperPurse.GetBalance() < price) return;

            
            bool success = shopperInventory.AddToFirstEmptySlotInventory(ability, 1);
            if (success)
            {
                AddToTransaction(ability, -1);
                stock[ability]--;
                shopperPurse.UpdateBalance(-price); //you can use - as a minus sign in math!
            }
        }
        private int FindFirstAbilitySlot(AbilityInventory shopperInventory, AbilityItem ability)
        {
            //Iterate through inventory
            //Find item that matches
            //return appropriate slot number, if cant find any return -1 (because -1 as a slot in an array doesnt exist)

            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetAbilityInSlot(i) == ability)
                {
                    return i;
                }
            }
            return -1;

        }
        public void AddToTransaction(AbilityItem ability, int quantity)
        {
            if (!transaction.ContainsKey(ability))         //We need to give this dictionary a default starting point. We check to see if the 
            {                                           //dictionary contains the key "item". If it doesn't, we want to give it a key item 
                transaction[ability] = 0;                  //and give it a default value. = 0 is a classic default value and makes the next code work.
            }

            int availability = GetAvailability(ability);
            if (transaction[ability] + quantity > availability)
            {
                transaction[ability] = availability;
            }
            else
            {
                transaction[ability] += quantity;              //transaction[item] allows us access to a specific item in the dictionary,
                                                            //and in turn gives us a quantity to deal with. We add to the quantity that was 
                                                            //passed into the method.   
            }
            if (transaction[ability] <= 0)
            {                                           //If the quantity of the transaction item goes below 0 in the transaction, 
                transaction.Remove(ability);               //we want to remove it from the dictionary because the lowest value is 0.
            }

            if (onChange != null)
            {
                onChange();
            }
        }
    }
}

