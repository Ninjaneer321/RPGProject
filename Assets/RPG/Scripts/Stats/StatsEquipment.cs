using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using Stats;
using UnityEngine;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, iModifierProvider
    {
        public IEnumerable<int> GetAdditiveModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as iModifierProvider;
                if (item == null) continue;

                foreach (int modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<int> GetPercentageModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as iModifierProvider;
                if (item == null) continue;

                foreach (int modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

    }

}
