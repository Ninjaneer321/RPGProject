using UnityEngine;
using GameDevTV.Saving;
using System.Collections;
using RPG.Core;
using RPG.Inventories;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        // CONFIG DATA
        [SerializeField] InventoryItem item = null;
        [Tooltip("Only modify if this spawner is going to spawn a Crafting Recipe.")]
        [SerializeField] CraftingRecipe collectableRecipe = null;
        [SerializeField] int number = 1;


        // LIFECYCLE METHODS
        private void Awake()
        {
            SpawnPickup();

        }

        // PUBLIC

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool isCollected() 
        { 
            return GetPickup() == null;
        }

        //PRIVATE

        private void SpawnPickup()
        {
            if (item != null)
            {
                var spawnedPickup = item.SpawnPickup(transform.position, number);
                spawnedPickup.transform.SetParent(transform);
            }
            if (collectableRecipe != null)
            {
                var spawnedPickup = collectableRecipe.SpawnPickup(transform.position, number);
                spawnedPickup.transform.SetParent(transform);
            }
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }

        }
        object ISaveable.CaptureState()
        {
            return isCollected();
        }

        void ISaveable.RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }
        }
    }
}