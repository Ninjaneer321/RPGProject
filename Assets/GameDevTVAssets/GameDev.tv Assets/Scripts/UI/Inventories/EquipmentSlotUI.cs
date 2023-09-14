using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using RPG.Stats;
using RPG.Core;
using GameDevTV.Utils;
using RPG.Abilities;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// An slot for the players equipment.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA

        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

        // CACHE
        Equipment playerEquipment;
        Inventory playerInventory;

        // LIFECYCLE METHODS
       
        private void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponent<Equipment>();
            playerInventory = player.GetComponent<Inventory>();
            playerEquipment.equipmentUpdated += RedrawUI;
        }

        private void Start() 
        {
            RedrawUI();
        }

        // PUBLIC

        public EquipLocation GetEquipLocation()
        {
            return equipLocation;
        }
        public int MaxAcceptable(InventoryItem item)
        {
            {
                EquipableItem equipableItem = item as EquipableItem;
                if (equipableItem == null) return 0;
                if (!equipableItem.CanEquip(equipLocation, playerEquipment)) return 0;
                if (GetItem() != null) return 0;
                if ((equipLocation == EquipLocation.Weapon) && item is WeaponConfig weaponConfig)
                {
                    if (weaponConfig.isTwoHanded && (playerEquipment.GetItemInSlot(EquipLocation.OffHand) != null))
                        return 0;
                }

                if (equipLocation == EquipLocation.OffHand)
                {
                    EquipableItem mainHand = playerEquipment.GetItemInSlot(EquipLocation.Weapon);
                    if (mainHand is WeaponConfig mainHandConfig)
                    {
                        if (mainHandConfig.isTwoHanded) return 0;
                    }
                }
                //The line below only makes sense after completing the Shops and Abilities course
                return 1;
            }
        }

        public void AddItems(InventoryItem item, int number)
        {
            playerEquipment.AddItem(equipLocation, (EquipableItem)item);


            //FILTER OUT ITEMS BY LEVEL 
            //BaseStats stats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            //int playerLevel = stats.GetLevel();

            //if (playerLevel < item.GetRequiredLevel())
            //{
            //    Debug.Log("Player level is too low");
            //    playerInventory.AddToFirstEmptySlot(item, number);
            //    return;              
            //}

            //else
            //{
            //    playerEquipment.AddItem(equipLocation, (EquipableItem)item);
            //} 

        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
        }
        public Ability GetAbility()
        {
            return null;
        }
        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        // PRIVATE

        void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
        }

    }
}