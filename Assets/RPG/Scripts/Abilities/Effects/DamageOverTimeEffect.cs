using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Damage Over Time Effect", menuName = "Abilities/Effects/Damage Over Time Effect", order = 0)]
    public class DamageOverTimeEffect : EffectStrategy
    {
        public float timeBetweenIntervals;
        public float repeats;
        public float healthChange; 

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (var target in data.GetTargets())
            {
                target.AddComponent<HealthChangeEffector>().Apply(data.GetUser(), healthChange, timeBetweenIntervals, repeats);
            }
            finished();
        }
    }
}
