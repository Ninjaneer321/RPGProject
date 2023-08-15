using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Utils;
using RPG.Combat;
using RPG.Control;
using RPG.Stats;
using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Abilities/Make New Ability", order = 0)]
    public class Ability : ActionInventoryItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float cooldownTime;
        [SerializeField] float manaCost = 0;
        [SerializeField] bool hasCastTime = false;
        [SerializeField] float castTime = 1f;
        [NonSerialized] private float timeSpentCasting = 0f;
        [SerializeField] public bool isBeingCasted = false;
        [SerializeField] Transform summonCirclePrefab = null;
        [SerializeField] float abilityRange = 0f;



        private SpellBar spellBar;
        Animator animator;
        PlayerManager playerManager;
        [SerializeField] float summonCircleDestroyDelay;


        //
        //
        //
        //PLAYER USAGE

        private float DistanceBetweenPlayerAndTarget()
        {
            Vector3 playerLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector3 targetLocation = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>().GetTarget().transform.position;

            return Vector3.Distance(playerLocation, targetLocation);
        }

        public override void Use(GameObject user)
        {
            BaseStats stats = user.GetComponent<BaseStats>();
            int playerLevel = stats.GetLevel();
            ActionInventoryItem item = this;

            if (!item.CanUseAbility(user.GetComponent<TraitStore>()))
            {
                return;
            }
            //if (playerLevel < item.GetRequiredLevel())
            //{
            //    Debug.Log("Player level is too low");
            //    return;
            //}
            if (abilityRange > 0f)
            {
                if (DistanceBetweenPlayerAndTarget() > abilityRange)
                {
                    Debug.Log("Player is too far");
                    return;
                }
            }

            if (isBeingCasted) return;
            Mana mana = user.GetComponent<Mana>();

            if (mana.GetMana() < manaCost)
            {
                return;
            }
            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemainingInventoryItem(this) > 0)
            {
                return;
            }

            AbilityData data = new AbilityData(user);


            if (hasCastTime)
            {
                if (!isBeingCasted)
                {
                    isBeingCasted = true;
                    data.GetUser().GetComponent<PlayerManager>().isCastingAbility = true; //Here
                    data.StartCoroutine(SpellCastCoroutine(data.GetUser()));
                }
            }
            else
            {
                targetingStrategy.StartTargeting(data, () => TargetAcquired(data));
            }

        }
        public IEnumerator SpellCastCoroutine(GameObject user)
        {

            animator = user.GetComponent<Animator>();
            AbilityData data = new AbilityData(user);
            spellBar = user.GetComponentInChildren<SpellBar>();
            spellBar.spellbarGameobject.SetActive(true);
            spellBar.slider.maxValue = castTime;
            spellBar.slider.value = 0f;

            //user.transform.LookAt(user.GetComponent<Fighter>().GetTarget().transform);

            //Transform summonCircle = Instantiate(summonCirclePrefab);
            //summonCircle.position = user.transform.position + (Vector3.up * .25f);

            while (timeSpentCasting < castTime)
            {
                animator.SetBool("castingMagic", true);
                if (data.GetUser().GetComponent<PlayerLocomotion>().moveDirection != Vector3.zero)
                {
                    //Destroy(summonCircle.gameObject);
                    timeSpentCasting = 0f;
                    spellBar.slider.value = 0f;
                    spellBar.spellbarGameobject.SetActive(false);
                    isBeingCasted = false;
                    data.GetUser().GetComponent<PlayerManager>().isCastingAbility = false;
                    animator.SetBool("castingMagic", false);
                    yield break;
                }

                spellBar.spellName.text = GetDisplayName();
                timeSpentCasting += Time.deltaTime;
                spellBar.slider.value += Time.deltaTime;
                yield return null;
            }
            animator.SetBool("castingMagic", false);
            spellBar.spellbarGameobject.SetActive(false);
            targetingStrategy.StartTargeting(data, () => TargetAcquired(data));
            yield return new WaitForSeconds(summonCircleDestroyDelay);
            //Destroy(summonCircle.gameObject);
            spellBar.slider.value = 0f;
            spellBar.slider.maxValue = 1f;
            timeSpentCasting = 0f;
            isBeingCasted = false;
            data.GetUser().GetComponent<PlayerManager>().isCastingAbility = false;

        }
        private void TargetAcquired(AbilityData data)
        {
            Mana mana = data.GetUser().GetComponent<Mana>();
            if (!mana.UseMana(manaCost)) return;
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldownInventoryItem(this, cooldownTime);
            foreach (var filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }
            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }

        }

        //
        //
        //
        //ENEMY USAGE

        public override void EnemyUse(GameObject enemy)
        {
            BaseStats stats = enemy.GetComponent<BaseStats>();
            int enemyLevel = stats.GetLevel();
            AbilityData data = new AbilityData(enemy);
            ActionInventoryItem item = this;


            if (!item.EnemyCanUseAbility(enemy.GetComponent<ActionStore>()))
            {
                return;
            }
            //if (enemyLevel < item.GetRequiredLevel())
            //{
            //    Debug.Log("Player level is too low");
            //    return;
            //}

            Vector3 enemyLocation = enemy.transform.position;
            Vector3 playerLocation = GameObject.FindGameObjectWithTag("Player").transform.position;

            if (abilityRange > 0f)
            {
                if (Vector3.Distance(enemyLocation, playerLocation) > abilityRange)
                {
                    Debug.Log("Enemy is too far");
                    return;
                }
            }
            CooldownStore cooldownStore = enemy.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemainingInventoryItem(this) > 0)
            {
                return;
            }
            targetingStrategy.EnemyStartTargeting(data, () => EnemyTargetAcquired(data));

        }


        private void EnemyTargetAcquired(AbilityData data)
        {
            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldownInventoryItem(this, cooldownTime);
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
            Debug.Log("Effect Finished!");
        }

    }
}
