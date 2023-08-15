using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> cooldownTimersInventoryItems = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialCooldownTimesInventoryItems = new Dictionary<InventoryItem, float>();

        Dictionary<AbilityItem, float> cooldownTimersAbilityItems = new Dictionary<AbilityItem, float>();
        Dictionary<AbilityItem, float> initialCooldownTimesAbilityItems = new Dictionary<AbilityItem, float>();

        private void Update()
        {
            var inventoryKeys = new List<InventoryItem>(cooldownTimersInventoryItems.Keys); //need copy of dictionary into list before we iterate over
            var abilityKeys = new List<AbilityItem>(cooldownTimersAbilityItems.Keys);
            foreach (InventoryItem item in inventoryKeys)
            {
                cooldownTimersInventoryItems[item] -= Time.deltaTime;
                if (cooldownTimersInventoryItems[item] < 0)
                {
                    cooldownTimersInventoryItems.Remove(item);
                    initialCooldownTimesInventoryItems.Remove(item);
                }
            }
            foreach (AbilityItem ability in abilityKeys)
            {
                cooldownTimersAbilityItems[ability] -= Time.deltaTime;
                if (cooldownTimersAbilityItems[ability] < 0)
                {
                    cooldownTimersAbilityItems.Remove(ability);
                    initialCooldownTimesAbilityItems.Remove(ability);
                }
            }

        }
        public void StartCooldownInventoryItem(InventoryItem ability, float cooldownTime)
        {
            cooldownTimersInventoryItems[ability] = cooldownTime;
            initialCooldownTimesInventoryItems[ability] = cooldownTime;
        }

        public float GetTimeRemainingInventoryItem(InventoryItem ability)
        {
            if (!cooldownTimersInventoryItems.ContainsKey(ability))
            {
                return 0;
            }

            return cooldownTimersInventoryItems[ability];
        }

        public float GetFractionRemainingInventoryItem(InventoryItem ability)
        {
            if (ability == null)
            {
                return 0;
            }
            if (!cooldownTimersInventoryItems.ContainsKey(ability))
            {
                return 0;
            }

            return cooldownTimersInventoryItems[ability] / initialCooldownTimesInventoryItems[ability];
        }


        public void StartCooldownAbilityItem(AbilityItem ability, float cooldownTime)
        {
            cooldownTimersAbilityItems[ability] = cooldownTime;
            initialCooldownTimesAbilityItems[ability] = cooldownTime;
        }

        public float GetTimeRemainingAbilityItem(AbilityItem ability)
        {
            if (!cooldownTimersAbilityItems.ContainsKey(ability))
            {
                return 0;
            }

            return cooldownTimersAbilityItems[ability];
        }

        public float GetFractionRemainingAbilityItem(AbilityItem ability)
        {
            if (ability == null)
            {
                return 0;
            }
            if (!cooldownTimersAbilityItems.ContainsKey(ability))
            {
                return 0;
            }

            return cooldownTimersAbilityItems[ability] / initialCooldownTimesAbilityItems[ability];
        }
    }
}
