using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Inventories
{
    public class ItemRemover : MonoBehaviour
    {
        [SerializeField] private InventoryItem itemToRemove;
        [SerializeField] private int number;

        public void RemoveItem()
        {
            Inventory.GetPlayerInventory().RemoveItem(itemToRemove, number);
        }
    }
}
