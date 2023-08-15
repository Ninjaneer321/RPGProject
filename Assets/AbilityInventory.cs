using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class AbilityInventory : MonoBehaviour, ISaveable
    {
        // STATE
        public AbilitySlot[] slots;
        [SerializeField] public int abilityInventorySize = 100;
        public event Action inventoryUpdated;
        private void Awake()
        {
            slots = new AbilitySlot[abilityInventorySize];
        }
        public struct AbilitySlot
        {
            public AbilityItem ability;
            public int number;
        }
        [System.Serializable]
        private struct AbilitySlotRecord
        {
            public string itemID;
            public int number;
        }

        public static AbilityInventory GetPlayerAbilityInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<AbilityInventory>();
        }
        public bool HasSpaceFor(AbilityItem item)
        {
            return FindInventorySlot(item) >= 0;
        }
        public int FindInventorySlot(AbilityItem item)
        {
            int i = FindInventoryStack(item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }
        public int FindInventoryStack(AbilityItem ability)
        {
            if (!ability.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].ability, ability))
                {
                    return i;
                }
            }
            return -1;
        }
        public int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].ability == null)
                {
                    return i;
                }
            }
            return -1;
        }
        public bool HasSpaceFor(IEnumerable<AbilityItem> abilities)
        {
            int freeSlots = FreeSlots();
            List<AbilityItem> stackedItems = new List<AbilityItem>();
            foreach (var item in abilities)
            {
                // Already have it inventory
                if (item.IsStackable())
                {
                    if (HasItem(item)) continue;
                    if (stackedItems.Contains(item)) continue;
                    stackedItems.Add(item);
                }
                // Already seen in the list
                if (freeSlots <= 0) return false;
                freeSlots--;
            }
            return true;
        }
        public bool HasItem(AbilityItem ability)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].ability, ability))
                {
                    return true;
                }
            }
            return false;
        }
        public int FreeSlots()
        {
            int count = 0;
            foreach (AbilitySlot slot in slots)
            {
                if (slot.number == 0)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetSize()
        {
            if (!gameObject.CompareTag("Player"))
            {
                return GetComponent<OtherInventorySpawner>().numberOfSlotsToSpawn;
            }
            return slots.Length;
        }
        public bool AddToFirstEmptySlotInventory(AbilityItem ability, int number)
        {

            foreach (var store in GetComponents<IItemStore>())
            {
                number -= store.AddAbilityItems(ability, number);
            }

            if (number <= 0) return true;

            int i = FindInventorySlot(ability);

            if (i < 0)
            {
                return false;
            }

            slots[i].ability = ability;
            slots[i].number += number;

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

            return true;
        }
        public AbilityItem GetAbilityInSlot(int slot)
        {
            return slots[slot].ability;
        }

        public int GetAbilitySlot(AbilityItem ability, int number)
        {
            // Loop through all of the inventory slots. 
            for (int i = 0; i < slots.Length; i++)
            {
                // Check if the current iteration's item equals to the parameter item.
                // Check if the current iteration's item number equals to or greater than the parameter number.
                if (object.ReferenceEquals(slots[i].ability, ability) && GetNumberInSlot(i) >= number)
                {
                    // If true, return the slot number
                    return i;
                }
            }
            // Return -1 by default.
            // When using this method to check if a slot is found, we simply check if the returned value equals to or greater than 0.
            return -1;
        }

        /// <summary>
        /// Get the number of items in the given slot.
        /// </summary>
        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
        }
        public void RemoveFromSlot(int slot, int number)
        {
            slots[slot].number -= number;
            if (slots[slot].number <= 0)
            {
                slots[slot].number = 0;
                slots[slot].ability = null;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }

        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="ability">The item type to add.</param>
        /// <param name="number">The number of items to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddAbilityToSlot(int slot, AbilityItem ability, int number)
        {
            if (slots[slot].ability != null)
            {
                return AddToFirstEmptySlotInventory(ability, number);
            }

            var i = FindInventoryStack(ability);
            if (i >= 0)
            {
                slot = i;
            }

            slots[slot].ability = ability;
            slots[slot].number += number;
            if (inventoryUpdated != null)
            {
                inventoryUpdated();

            }

            return true;

        }
        public void RemoveAbility(AbilityItem ability, int number)
        {
            if (ability == null) return;
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].ability, ability))
                {
                    slots[i].number -= number;
                    if (slots[i].number <= 0)
                    {
                        slots[i].ability = null;
                        slots[i].number = 0;
                    }
                    if (inventoryUpdated != null)
                    {
                        inventoryUpdated();
                    }
                    return;
                }
            }
        }

        public void RemoveAbility(string itemID, int number)
        {
            AbilityItem ability = AbilityItem.GetFromID(itemID);
            RemoveAbility(ability, number);
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }
        object ISaveable.CaptureState()
        {
            var slotStrings = new AbilitySlotRecord[abilityInventorySize];
            for (int i = 0; i < abilityInventorySize; i++)
            {
                if (slots[i].ability != null)
                {
                    slotStrings[i].itemID = slots[i].ability.GetItemID();
                    slotStrings[i].number = slots[i].number;
                }
            }
            return slotStrings;
        }

        void ISaveable.RestoreState(object state)
        {
            var slotStrings = (AbilitySlotRecord[])state;
            for (int i = 0; i < abilityInventorySize; i++)
            {
                slots[i].ability = AbilityItem.GetFromID(slotStrings[i].itemID);
                slots[i].number = slotStrings[i].number;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }
    }

}
