using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = "Abilities/Targeting/Demo", order = 0)]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerManager playerManager = data.GetUser().GetComponent<PlayerManager>();
            playerManager.StartCoroutine(Targeting(data, finished));
        }

        public override void EnemyStartTargeting(AbilityData data, Action finished)
        {
            AIController aiController = data.GetUser().GetComponent<AIController>();
            aiController.StartCoroutine(Targeting(data, finished));
        }

        private IEnumerator Targeting(AbilityData data, Action finished)
        {
            while (true)
            {
               if (data.GetUser().GetComponent<Fighter>().target == null)
               {
                   yield break;
               }

               Debug.Log("Target Locked On");
               data.SetTargets(GetEnemies(data.GetUser()));
               data.SetTargetedPoint(data.GetUser().GetComponent<Fighter>().target.transform.position);
               finished();
               yield break;
            }
                        
        }

        private IEnumerable<GameObject> GetEnemies(GameObject user)
        {
            Fighter fighter = user.GetComponent<Fighter>();
            yield return fighter.GetTarget().gameObject;
        }
    }
}
