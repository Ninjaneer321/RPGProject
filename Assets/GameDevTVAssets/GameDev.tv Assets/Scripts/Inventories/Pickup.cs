using System.Collections;
using RPG.Inventories;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        InventoryItem item;
        int number = 1;
        [SerializeField] CraftingRecipe collectableRecipe;
        [SerializeField] bool isRespawning = false;
        [SerializeField] public float respawnTime = 5;
        [SerializeField] CraftingRecipeBank craftingRecipeBank;

        // CACHED REFERENCE
        Inventory inventory;
        OtherInventorySpawner otherInventorySpawner;

        public GameObject lootBeam;


        // LIFECYCLE METHODS

        private void Awake()
        {
            otherInventorySpawner = gameObject.GetComponent<OtherInventorySpawner>();

            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();

            if (otherInventorySpawner == null) return;
            if (otherInventorySpawner.lootBeam != null)
            {
                lootBeam = Instantiate(otherInventorySpawner.lootBeam, transform);
            }

        }


        // PUBLIC

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(InventoryItem item, int number)
        {
            this.item = item;
            if (!item.IsStackable())
            {
                number = 1;
            }
            this.number = number;
        }

        public void SetupForRecipe(CraftingRecipe recipe, int number)
        { 
            this.collectableRecipe = recipe;
            this.number = number;
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetNumber()
        {
            return number;
        }

        public void PickupItem()
        {
            //PLAYER LOOT ANIMATION

            if (item != null)
            {
                string newItemString = "<br>Item received: " + item.GetDisplayName() + ". x:" + number.ToString() + ".";
                ChatBox chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();
                chatBox.UpdateText(newItemString);


                bool foundSlot = inventory.AddToFirstEmptySlotInventory(item, number);

                if (foundSlot)
                {
                    if (!isRespawning)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    else
                    {
                        StartCoroutine(HideForSeconds(respawnTime));
                    }
                }
            }

            if (collectableRecipe != null)
            {
                string newItemString = "<br>Item received: " + collectableRecipe.GetDisplayName() + ". x:" + number.ToString() + ".";
                ChatBox chatBox = GameObject.FindGameObjectWithTag("Player").GetComponent<ChatBox>();
                chatBox.UpdateText(newItemString);

                //CraftingRecipeBank.AddNewCraftingRecipes(collectableRecipe)
                //CraftingRecipeBank is stored on the Crafting UI GameObject
                //CraftingUI.GetCraftingRecipeBank


                craftingRecipeBank.AddNewCraftingRecipes(collectableRecipe);

                if (!isRespawning)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    StartCoroutine(HideForSeconds(respawnTime));
                }

                //bool foundSlot = inventory.AddToFirstEmptySlot(collectableRecipe, number);

                //if (foundSlot)
                //{
                //    if (!isRespawning)
                //    {
                //        Destroy(gameObject);
                //        return;
                //    }
                //    else
                //    {
                //        StartCoroutine(HideForSeconds(respawnTime));
                //    }
                //}
            }


        }

        public void PickupRespawnCoroutine()
        {
            StartCoroutine(HideForSeconds(respawnTime));
        }
        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        public void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item);
        }
    }
}