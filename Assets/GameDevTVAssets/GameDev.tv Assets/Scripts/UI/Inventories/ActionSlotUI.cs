using System.Collections;
using System.Collections.Generic;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using RPG.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// The UI slot for the player action bar.
    /// </summary>
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>, IDragContainer<Ability>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon inventoryItemIcon = null;
        [SerializeField] AbilityItemIconActionSlot abilityItemIcon = null;
        [SerializeField] int index = 0;
        [SerializeField] Image cooldownOverlay = null;
        // CACHE
        ActionStore store;
        CooldownStore cooldownStore;

        // LIFECYCLE METHODS
        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            store = player.GetComponent<ActionStore>();
            cooldownStore = player.GetComponent<CooldownStore>();
            store.storeUpdated += UpdateIcon;
        }

        private void Update()
        {
            cooldownOverlay.fillAmount = cooldownStore.GetFractionRemainingInventoryItem(GetItem());
        }

        // PUBLIC

        public void AddItems(InventoryItem item, int number)
        {
            Debug.Log("Add Inventory Item");
            store.AddAction(item, index, number);
        }
        public void AddItems(Ability ability, int number)
        {
            store.AddAction(ability, index, number);
        }
        public InventoryItem GetItem()
        {
            return store.GetAction(index);
        }
        public Ability GetAbility()
        {
            return store.GetAbility(index);
        }

        Ability IDragSource<Ability>.GetItem()
        {
            return store.GetAbility(index);
            //Debug.Log("Not implemented");
            //throw new System.NotImplementedException();
        }

        public int GetNumber()
        {
            return store.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return store.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            store.RemoveItems(index, number);
        }

        // PRIVATE

        void UpdateIcon()
        {
            if (GetItem())
            {
                inventoryItemIcon.SetItem(GetItem(), GetNumber());
                return;
            }

            abilityItemIcon.SetItem(GetAbility(), 1);
        }

        public int MaxAcceptable(Ability ability)
        {
            return store.MaxAcceptable(ability, index);
        }




    }
}
