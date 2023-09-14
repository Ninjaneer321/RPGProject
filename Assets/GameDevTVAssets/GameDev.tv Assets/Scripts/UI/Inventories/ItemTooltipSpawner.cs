using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Core.UI.Tooltips;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        //public override bool CanCreateTooltip()
        //{
        //    var item = GetComponent<IItemHolder>().GetItem();
        //    if (!item) return false;

        //    return true;
        //}
        public override bool CanCreateTooltip()
        {
            var item = GetComponent<IItemHolder>().GetItem();
            //if item is false, we should get the .GetAbility().
            if (!item)
            {
                var ability = GetComponent<IItemHolder>().GetAbility();
                if (!ability && !item) return false;
            }

            return true;
        }
        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();
            if (!itemTooltip) return;

            var item = GetComponent<IItemHolder>().GetItem();
            if (!item)
            {
                var ability = GetComponent<IItemHolder>().GetAbility();
                itemTooltip.Setup(ability);
            }
            else
            {
                itemTooltip.Setup(item);
            }

        }
        //public override void UpdateTooltip(GameObject tooltip)
        //{
        //    var itemTooltip = tooltip.GetComponent<ItemTooltip>();
        //    if (!itemTooltip) return;

        //    var item = GetComponent<IItemHolder>().GetItem();

        //    itemTooltip.Setup(item);
        //}
    }
}