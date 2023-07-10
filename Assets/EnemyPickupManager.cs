using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

public class EnemyPickupManager : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] OtherInventorySpawner otherInventorySpawner;
    [SerializeField] OtherInventory otherInventory;

    [SerializeField] public bool isLooted = false;

    private void OnEnable()
    {
        isLooted = false;
        //inventory.inventorySize = otherInventorySpawner.numberOfSlotsToSpawn;
    }
    private void Update()
    {

        if (inventory.FreeSlots() == inventory.slots.Length && isLooted == true)
        {
            GetComponentInParent<Health>().DeathDelayCoroutine();
            otherInventorySpawner.numberOfSlotsToSpawn = 0;
        }
    }

    public void SetIsLooted()
    {
        isLooted = true;
    }
}
