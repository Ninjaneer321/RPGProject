using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.Animations;
using Stats;
using GameDevTV.Utils;
using UnityEngine.Events;
using System;
using RPG.Combat;
using RPG.UI;
using RPG.Movement;
using GameDevTV.Inventories;
using RPG.Inventories;
using RPG.Control;

namespace RPG.Stats
{
    public class Health : MonoBehaviour, ISaveable
    {

        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] float healthRestoreFraction = 1f;
        [SerializeField] HealthBar healthBar;
        [SerializeField] float healthpoints = 0;
        [SerializeField] public bool isDead;

        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] TakeHealingEvent takeHealing;
        public UnityEvent onDie;

        [SerializeField] public float deathDelayTime = 10f;

        BaseStats baseStats;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
           
        }

        [System.Serializable]
        public class TakeHealingEvent : UnityEvent<float>
        {

        }

        Animator animator;

        // Start is called before the first frame update


        private void Start()
        {
           animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            GetInitialHealth();
        }

        //private void Awake()
        //{
        //    animator = GetComponent<Animator>();
        //    baseStats = GetComponent<BaseStats>();
        //    GetInitialHealth();

        //}


        private float GetInitialHealth()
        { 
            return healthpoints = baseStats.GetStat(Stat.Health);
        }



        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }


        // Update is called once per frame

        void Update()
        {
                if (healthBar != null)
                {
                    healthBar.valueText.text = string.Format("{0:0}/{1:0}", GetHealthPoints(), GetMaxHealthPoints());
                }

            if (this.gameObject.tag == "Player")
            {
                if (healthpoints > baseStats.GetStat(Stat.Health))
                {
 
                    healthpoints = GetMaxHealthPoints();
                }

                //Health Regeneration Portion

                if (healthpoints < GetMaxHealthPoints())
                {
                    healthpoints += healthRestoreFraction * Time.deltaTime;

                }
            }

        }


        public float GetHealthPoints()
        {
            return healthpoints;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(Stat.Health);
        }


        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return healthpoints / baseStats.GetStat(Stat.Health);
        }

        private void RegenerateHealth()
        {
            float regenHealthPoints = baseStats.GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthpoints = Mathf.Max(healthpoints, regenHealthPoints);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void DealDamage(GameObject instigator, float damage)
        {
            //Dodge Code
            if (baseStats.GetStat(Stat.DodgeChance) >= UnityEngine.Random.Range(0, 100))
            {
                //Maybe play a UI element that says DODGE!
                Debug.Log("Dodge!");
                damage = 0;
            }

            healthpoints = Mathf.Max(healthpoints - damage, 0);
            takeDamage.Invoke(damage);
            SetTargetIfNoTarget(instigator);
            GetComponentInChildren<DamageTextSpawner>().Spawn(damage);

            if (healthpoints <= 0)
            {
                onDie.Invoke();
                AwardExperience(instigator);
                instigator.GetComponent<Fighter>().AwardSkillExperience();
                Die();
            }

        }

        public void SetTargetIfNoTarget(GameObject targetToSet)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Fighter playerFighter = player.GetComponent<Fighter>();

            if (playerFighter.GetTarget() != null) return;
            if (playerFighter.GetTarget() == null)
            {
                playerFighter.target = targetToSet.GetComponent<Health>();
            }
        }
        public void Heal(float healthToRestore)
        {
            healthpoints = Mathf.Min(healthpoints + healthToRestore, GetMaxHealthPoints());
            takeHealing.Invoke(healthToRestore);
        }

        public void ResetHealth()
        {
            healthpoints = GetMaxHealthPoints();
        }

        public void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }



        private void Die()
        {
            if (healthpoints <= 0)
            {
                isDead = true;
                Destroy(gameObject.GetComponent<CombatTarget>());
                if (gameObject.tag == "Enemy")
                {
                    gameObject.AddComponent<Pickup>();
                    GetComponent<Mover>().Stop();
                    gameObject.GetComponentInParent<EnemyRespawner>().Death = true;

                    //StartCoroutine(DeathDelay());  
                }
                animator.SetBool("dead", true);
  
            }
            //else animator.SetBool("dead", false);
        }

        public void DeathDelayCoroutine()
        {
            StartCoroutine(DeathDelay());
        }
        public IEnumerator DeathDelay()
        {
            Destroy(GetComponent<Pickup>().lootBeam.gameObject);
            yield return new WaitForSeconds(deathDelayTime);
            Destroy(gameObject);
        }

        public object CaptureState()
        {
            return healthpoints;
        }

        public void RestoreState(object state)
        {
            healthpoints = (float)state;

            if (healthpoints == 0)
            {
                Die();
            }
        }
    }

}
