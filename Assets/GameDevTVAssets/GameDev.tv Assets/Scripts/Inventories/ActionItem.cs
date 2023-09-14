using System;
using GameDevTV.Utils;
using RPG.Abilities;
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
        [SerializeField] float cooldownTime;
        [SerializeField] Condition usageCondition;
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;

        // PUBLIC

        /// <summary>
        /// Trigger the use of this item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual bool UseItem(GameObject user)
        {
            AbilityData data = new AbilityData(user);
            targetingStrategy.StartTargeting(data, () => TargetAcquired(data));
            return true;
        }
        private void TargetAcquired(AbilityData data)
        {
            ActionItem item = this;
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldownInventoryItem(item, cooldownTime);
            foreach (var filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }
            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }

        }
        private void EffectFinished()
        {
            {
                Debug.Log("Effect Finished!");
            }

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

