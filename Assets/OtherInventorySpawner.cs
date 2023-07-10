using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Inventories
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(OtherInventory))]
    public class OtherInventorySpawner : MonoBehaviour
    {
        [SerializeField] List<DropLibrary> dropLibraries = new List<DropLibrary>();
        [SerializeField] public GameObject lootBeam = null;
        public int numberOfSlotsToSpawn = 0;

        //NEED TO CALL THIS FROM CHEST PICKUP 
        public void AddItemsToOtherInventory()
        {
            Debug.Log("ADD ITEMS TO INVENTORY");

                var baseStats = GetComponent<BaseStats>();
                Inventory otherInventory = GetComponent<Inventory>();
                foreach (DropLibrary dropLibrary in dropLibraries)
                {
                    var drops = dropLibrary.GetRandomDrops(baseStats.GetLevel());
                   
                    foreach (var drop in drops)
                    {
                        numberOfSlotsToSpawn++;    //Increments by 1 for each drop dropped

                    if (drop.item.IsStackable())
                        {
                            if (otherInventory.HasItem(drop.item))
                            {
                                numberOfSlotsToSpawn--;
                            }
                        }

                        bool foundSlot = otherInventory.AddToFirstEmptySlot(drop.item, drop.number);

                        if (!foundSlot)
                        {
                            break;
                        }

                        enabled = false;
                    }
               }     

        }

    }
}


