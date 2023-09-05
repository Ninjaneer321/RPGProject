using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using RPG.Abilities;
using RPG.UI.Shops;
using UnityEngine;

namespace RPG.Shops

{
    public class ShopItemHolder : MonoBehaviour, IItemHolder

    {
        //public AbilityItem GetAbility()
        //{
        //    return gameObject.GetComponentInParent<AbilityRowUI>().GetShopAbility().GetAbilityItem();
        //}

        public InventoryItem GetItem()

        {
            return gameObject.GetComponentInParent<RowUI>().GetShopItem().GetInventoryItem();

        }
        public Ability GetAbility()
        {
            Debug.Log("Not implemented");
            throw new System.NotImplementedException();
        }

    }

}