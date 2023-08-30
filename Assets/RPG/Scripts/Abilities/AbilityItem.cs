using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

public abstract class AbilityItem : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
    [SerializeField] public string itemID = null;
    [Tooltip("Item name to be displayed in UI.")]
    [SerializeField] string displayName = null;
    [Tooltip("Item description to be displayed in UI.")]
    [SerializeField] [TextArea] string description = null;
    [Tooltip("The UI icon to represent this item in the inventory.")]
    [SerializeField] Sprite icon = null;
    [SerializeField] float price;
    [Tooltip("The price of the item in a shop, without any changes.")]
    [SerializeField] ItemCategory category = ItemCategory.None;
    [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
    [SerializeField] bool stackable = false;


    static Dictionary<string, AbilityItem> abilityLookupCache;

    public static AbilityItem GetFromID(string itemID)
    {
        if (abilityLookupCache == null)
        {
            abilityLookupCache = new Dictionary<string, AbilityItem>();
            var itemList = Resources.LoadAll<AbilityItem>("");
            foreach (var item in itemList)
            {

                if (abilityLookupCache.ContainsKey(item.itemID))
                {
                    //commented out because it was breaking item quests??


                    //Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                    //continue;
                }

                abilityLookupCache[item.itemID] = item;
            }
        }

        if (itemID == null || !abilityLookupCache.ContainsKey(itemID)) return null;
        return abilityLookupCache[itemID];
    }

    public ItemCategory GetCategory()
    {
        return category;
    }
    public string GetDisplayName()
    {
        return displayName;
    }
    public string GetDescription()
    {
        return description;
    }
    public Sprite GetIcon()
    {
        return icon;
    }
    public bool IsStackable()
    {
        return stackable;
    }
    public float GetPrice()
    {
        return price;
    }

    public string GetItemID()
    {
        return itemID;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Generate and save a new UUID if this is blank.
        if (string.IsNullOrWhiteSpace(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // Require by the ISerializationCallbackReceiver but we don't need
        // to do anything with it.
    }
}
