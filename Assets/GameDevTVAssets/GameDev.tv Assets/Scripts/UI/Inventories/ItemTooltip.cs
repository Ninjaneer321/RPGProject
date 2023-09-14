using UnityEngine;
using TMPro;
using GameDevTV.Inventories;
using RPG.Abilities;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;

        // PUBLIC

        public void Setup(InventoryItem item)
        {
            titleText.text = item.GetDisplayName();
            bodyText.text = item.GetDescription();
        }
        
        public void Setup(Ability ability)
        {
            titleText.text = ability.GetDisplayName();
            bodyText.text = ability.GetDescription();
        }
    }
}
