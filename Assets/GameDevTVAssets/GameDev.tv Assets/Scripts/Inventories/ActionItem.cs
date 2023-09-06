using System;
using GameDevTV.Utils;
using RPG.Stats;
using Stats;
using UnityEngine;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// An inventory item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    [CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Action Item"))]
    public class ActionItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Does an instance of this item get consumed every time it's used.")]
        [SerializeField] bool consumable = false;
        [SerializeField] Condition usageCondition;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual bool UseItem(GameObject user)
        {
            Debug.Log("Using action: " + this);
            return false;
        }

        public virtual void EnemyUse(GameObject user) 
        {
            
        }

        public bool isConsumable()
        {
            return consumable;
        }



        public bool CanUseAbility(TraitStore traitStore)
        {
            return usageCondition.Check(traitStore.GetComponents<IPredicateEvaluator>());
        }

        public bool EnemyCanUseAbility(ActionStore actionStore)
        {
            return usageCondition.Check(actionStore.GetComponents<IPredicateEvaluator>());
        }
        
        
    }
}

