using System;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Abilities;
using Stats;
using RPG.Stats;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// Provides the storage for an action bar. The bar has a finite number of
    /// slots that can be filled and actions in the slots can be "used".
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class ActionStore : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        // STATE
         Dictionary<int, DockedItemSlot> dockedItems = new Dictionary<int, DockedItemSlot>();
        private class DockedItemSlot 
        {
            public ActionItem item;
            public Ability ability;
            public int number;
        }
        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action storeUpdated;

        /// <summary>
        /// Get the action at the given index.
        /// </summary>
        public ActionItem GetAction(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].item;
            }
            return null;
        }
        
        public Ability GetAbility(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].ability;
            }
            return null;
        }

        /// <summary>
        /// Get the number of items left at the given index.
        /// </summary>
        /// <returns>
        /// Will return 0 if no item is in the index or the item has
        /// been fully consumed.
        /// </returns>
        public int GetNumber(int index)
        {
            if (dockedItems.ContainsKey(index))
            {
                return dockedItems[index].number;
            }
            return 0;
        }

        public void AddAction(Ability ability, int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                if (object.ReferenceEquals(ability, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }

            else
            {
                var slot = new DockedItemSlot();
                slot.item = null;
                slot.ability = ability;
                slot.number = number;
                dockedItems[index] = slot;
            }
            if (storeUpdated != null)
            {
                storeUpdated();
            }

        }
        /// <summary>
        /// Add an item to the given index.
        /// </summary>
        /// <param name="item">What item should be added.</param>
        /// <param name="index">Where should the item be added.</param>
        /// <param name="number">How many items to add.</param>
        public void AddAction(InventoryItem item, int index, int number)
        {
            Debug.Log("AddActionInventoryItem in ActionStore.cs");
            if (dockedItems.ContainsKey(index))
            {
               if (object.ReferenceEquals(item, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }

            else
            {
                var slot = new DockedItemSlot();
                //slot.ability = null;
                slot.item = item as ActionItem;
                slot.number = number;
                dockedItems[index] = slot;
            }
            if (storeUpdated != null)
            {
                storeUpdated();
            }
            //If exists
                //  add to existing
            // Else
                //Create DockedItemsList
                //Setup DockedItemSlot
                //Add to the dictionary
            // Update the UI

        }

        /// <summary>
        /// Use the item at the given slot. If the item is consumable one
        /// instance will be destroyed until the item is removed completely.
        /// </summary>
        /// <param name="user">The character that wants to use this action.</param>
        /// <returns>False if the action could not be executed.</returns>
        public bool UseItem(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                if (dockedItems[index].ability == null)
                {
                    Debug.Log("Ability is null. Using ActionItem");
                    dockedItems[index].item.UseItem(user);
                    if (dockedItems[index].item.isConsumable())
                    {
                        RemoveItems(index, 1);
                    }
                }

                return true;
            }
            return false;
        }

        public bool UseAbility(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                if (dockedItems[index].item == null)
                {
                    dockedItems[index].ability.UseAbility(user);
                    Debug.Log("Item is null. Using Ability");
                }
                return true;
            }
            return false;
        }

        public bool EnemyUse(int index, GameObject user)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].item.EnemyUse(user);
                if (dockedItems[index].item.isConsumable())
                {
                    RemoveItems(index, 1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a given number of items from the given slot.
        /// </summary>
        public void RemoveItems(int index, int number)
        {
            if (dockedItems.ContainsKey(index))
            {
                dockedItems[index].number -= number;
                if (dockedItems[index].number <= 0)
                {
                    dockedItems.Remove(index);
                }
                if (storeUpdated != null)
                {
                    storeUpdated();
                }
            }
            
        }

        /// <summary>
        /// What is the maximum number of items allowed in this slot.
        /// 
        /// This takes into account whether the slot already contains an item
        /// and whether it is the same type. Will only accept multiple if the
        /// item is consumable.
        /// </summary>
        /// <returns>Will return int.MaxValue when there is not effective bound.</returns>
        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;
            if (!actionItem) return 0;

            if (dockedItems.ContainsKey(index) && !object.ReferenceEquals(item, dockedItems[index].item))
            {
                if (dockedItems[index].item == null)
                {
                    var player = GameObject.FindWithTag("Player");
                    Inventory playerInventory = player.GetComponent<Inventory>();
                    playerInventory.RemoveItem(item, dockedItems[index].number);
                    dockedItems[index].ability = null;
                    dockedItems[index].item = actionItem;
                    Debug.Log("Item DID override the Ability. The UI just needs to be updated somehow.");
                    Debug.Log("However, the item duplicates in the Inventory when this happens. Need to figure that out.");
                    if (storeUpdated != null)
                    {
                        storeUpdated();
                    }
                    //Return something here.
                }
                return 0;
            }
            if (actionItem.isConsumable())
            {
                return int.MaxValue;
            }
            if (dockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }

        public int MaxAcceptable(Ability ability, int index)
        {
            var actionItem = ability as Ability;
            if (!actionItem) return 0;
            if (dockedItems.ContainsKey(index) && !object.ReferenceEquals(ability, dockedItems[index].ability))
            {       
                if (dockedItems[index].ability == null)
                {
                    var player = GameObject.FindWithTag("Player");
                    Inventory playerInventory = player.GetComponent<Inventory>();
                    playerInventory.AddToFirstEmptySlotInventory(dockedItems[index].item, dockedItems[index].number);
                    dockedItems[index].item = null;
                    dockedItems[index].ability = actionItem;
                    Debug.Log("Item DID override the Item. The UI just needs to be updated somehow");
                    Debug.Log("Item gets deleted. Needs to be moved to the player inventory");

                    if (storeUpdated != null)
                    {
                        storeUpdated();
                    }
                    //Return something here.
                }
                return 0;
            }
            if (dockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }
        /// PRIVATE

        [System.Serializable]
        private struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        object ISaveable.CaptureState()
        {
            var state = new Dictionary<int, DockedItemRecord>();

            //In the future, we want this to save and restore AbilityItems on the ActionStore as well.
            foreach (var pair in dockedItems)
            {
                var record = new DockedItemRecord();
                record.itemID = pair.Value.item.GetItemID();
                record.number = pair.Value.number;
                state[pair.Key] = record;
            }
            return state;
        }

        void ISaveable.RestoreState(object state)
        {
            dockedItems.Clear();
            var stateDict = (Dictionary<int, DockedItemRecord>)state;

            //In the future, we want this to save and restore AbilityItems on the ActionStore as well.
            foreach (var pair in stateDict)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);
            }
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {


            //if (predicate == EPredicate.MinimumHealthPercentageToUseAbility)
            //{
            //    if (int.TryParse(parameters[0], out int healthPercentageToTest))
            //    {
            //        if (health.GetPercentage() <= healthPercentageToTest)
            //        {
            //            return true;
            //        }
            //    }
            //    return false;
            //}
            //return null;

            Health health = gameObject.GetComponent<Health>();

            if (predicate == EPredicate.MinimumHealthPercentageToUseAbility)
            {
                if (int.TryParse(parameters[1], out int healthPercentage))
                {
                    Debug.Log(healthPercentage);

                    if (health.GetPercentage() <= healthPercentage)
                    {
                        return true;
                    }
                }
                return false;
            }
            return null;
        }
    }
}