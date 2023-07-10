using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Utils
{

    public enum EPredicate
    {
        Select,
        HasQuest,
        CompletedObjective,
        CompletedQuest,
        HasNotCompletedQuest,
        MinimumTrait,
        MinimumLevel,
        MinimumHealthPercentageToUseAbility,
        HasItem,
        HasItems,
        HasItemEquipped,
        HasKilled
    }
    public interface IPredicateEvaluator
   {
        bool? Evaluate(EPredicate predicate, string[] parameters); //? turns it into a "nullable" boolean
    }
}
