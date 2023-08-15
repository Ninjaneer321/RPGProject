using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class AbilityRowUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameField;

        [SerializeField] Image iconField;



        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI priceField;
        [SerializeField] TextMeshProUGUI quantityField;

        AbilityShop currentShop = null;
        ShopAbility ability = null;

        //We need to use a serialized field to get hold of the TextMeshPro component of the quantity 
        //field to populate it.
        public void Setup(AbilityShop currentShop, ShopAbility ability)
        {
            this.currentShop = currentShop;
            this.ability = ability;
            iconField.sprite = ability.GetIcon();
            nameField.text = ability.GetName();
            availabilityField.text = $"{ability.GetAvailability()}"; //item.GetAvailability();
            priceField.text = $"{ability.GetPrice()}";
            quantityField.text = $"{ability.GetQuantityInTransaction()}";  //we need to do this weird shit around the integer to make it readable 
                                                                        //by .text components. remember typing this when making different UI. 
        }

        public void Add()
        {
            currentShop.AddToTransaction(ability.GetAbilityItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(ability.GetAbilityItem(), -1);
        }

        public InventoryItem GetItem()
        {
            throw new NotImplementedException();

        }
        public ShopAbility GetShopAbility()
        {
            return ability;
        }

        public AbilityItem GetAbility()
        {
            return ability.GetAbilityItem();
        }
    }
}

