using UnityEngine;
using GameDevTV.Inventories; // Make sure to include the appropriate namespace
using static CraftingRecipeBank;

[CreateAssetMenu(fileName = "New Item Recipe", menuName = "RPG/Inventory/Item Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public InventoryItem item;
    public Ingredients[] ingredients;
    [Tooltip("The prefab that should be spawned when this item is dropped.")]
    [SerializeField] Pickup pickup = null;
    [SerializeField] int number;
    [SerializeField] string displayName;

    [System.Serializable]
    public class Ingredients
    {
        public InventoryItem item;
        public int number;
    }
    public Pickup SpawnPickup(Vector3 position, int number)
    {
        var pickup = Instantiate(this.pickup);
        pickup.transform.position = position;
        pickup.SetupForRecipe(this, number);
        return pickup;
    }

    public string GetDisplayName()
    {
        return displayName;
    }
}