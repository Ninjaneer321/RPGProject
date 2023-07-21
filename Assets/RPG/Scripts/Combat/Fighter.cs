using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using GameDevTV.Utils;
using GameDevTV.Saving;
using RPG.Stats;
using RPG.Control;
using RPG.Movement;
using Stats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using GameDevTV.Inventories;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        //WEAPON INFO
        [SerializeField] public Transform rightHandTransform = null;
        [SerializeField] public Transform leftHandTransform = null;

        public WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        [SerializeField] public WeaponConfig defaultWeapon = null;

        [SerializeField] public Health target;
        BaseStats baseStats;
        [SerializeField] float criticalDamageBonus = 2f;
        Equipment equipment;
        [SerializeField] public float timeSinceLastAttack = Mathf.Infinity; // new NetworkVariable<float>(Mathf.Infinity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [SerializeField] public ShieldConfig defaultShield = null;

        [SerializeField] public HitEvent hitEvent;

        [SerializeField] public bool isAggrevated = false;


        [SerializeField] public float skillExperienceToReward;

        [System.Serializable]
        public class HitEvent : UnityEvent 
        {

        }

        [SerializeField] public AggrevateEvent aggrevateEvent;
        [System.Serializable]
        public class AggrevateEvent : UnityEvent
        {

        }

        public ShieldConfig currentShieldConfig;
        LazyValue<Shield> currentShield;
        private void Awake()
        {
            baseStats = gameObject.GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            currentShieldConfig = defaultShield;
            currentShield = new LazyValue<Shield>(SetupDefaultShield);

            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }
        void Start()
        {
            currentWeapon.ForceInit();

            if (gameObject.GetComponent<PlayerManager>())
            {
                currentShield.ForceInit();
            }
        }

        private void Update()
        {
            if (target == null)
            {
                return;
            }
            if (target.GetComponent<Health>().IsDead()) return;
            if (GetComponent<Health>().IsDead()) return;
            if (!GetComponent<Mover>()) return;

            if (!GetIsInRange(target.transform))
            {
                if (isAggrevated == false)
                {
                    aggrevateEvent.Invoke();
                    isAggrevated = true;
                }
                GetComponent<Mover>().MoveTo(target.transform.position);

            }
            else
            {
                AttackBehavior();
                GetComponent<Mover>().Stop();
            }
        }

        private void FixedUpdate()
        {
            //timeSinceLastAttack += NetworkManager.Singleton.LocalTime.FixedDeltaTime;
            timeSinceLastAttack += Time.fixedDeltaTime;
            //gameObject.GetComponent<PlayerManager>().timeSinceLastAttack += Time.fixedDeltaTime;


        }

        public void AttackBehavior()
        {

            Vector3 lookVector = target.transform.position - transform.position;
            lookVector.y = 0;
            Quaternion rot = Quaternion.LookRotation(lookVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
            if (timeSinceLastAttack >= currentWeaponConfig.weaponSpeed)
            {
                TriggerAttack();
                timeSinceLastAttack = 0.0f;
            }

        }

        public void AwardSkillExperience()
        {
            //THIS IS ACTUALLY TAKING MY PLAYER FIGHTER COMPONENT'S SKILLEXPERIENCEREWARD AND NOT THE ENEMIES

            SkillExperience skillExperience = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillExperience>();
            Debug.Log(skillExperienceToReward);
            Debug.Log(target.GetComponent<Fighter>().skillExperienceToReward);
            skillExperience.GainExperience(currentWeaponConfig.weaponSkill, target.GetComponent<Fighter>().skillExperienceToReward);

        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //THIS IS AI ATTACK AND GETS CALLED BY AICONTROLLER 
        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
        }
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private Shield SetupDefaultShield()
        {
            return AttachShield(defaultShield);
        }
        private void UpdateWeapon()
        {

            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            var shield = equipment.GetItemInSlot(EquipLocation.OffHand) as ShieldConfig;

            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }

            if (shield != null)
            {
                EquipShield(shield);

            }
            else
            {
                DestroyOffHandItem();
            }
        }
        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }
        public void EquipShield(ShieldConfig shield)
        {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("hasShield", true);

            currentShieldConfig = shield;
            currentShield.value = AttachShield(shield);
        }


        // use to destroy Shield/offhand prefab when the item is removed
        private void DestroyOffHandItem()
        {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("hasShield", false);

            Shield offHandWeapon = leftHandTransform.GetComponentInChildren<Shield>();
            if (offHandWeapon == null)
            {
                return;
            }
            Destroy(offHandWeapon.gameObject);

        }


        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetRange();
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }
        private Shield AttachShield(ShieldConfig shield)
        {
            Animator animator = GetComponent<Animator>();
            return shield.SpawnEquipableItem(rightHandTransform, leftHandTransform, animator);
        }


        //[ServerRpc (RequireOwnership = false)]
        //public void HitTestServerRpc()
        //{
        //    timeSinceLastAttack = 0.0f;
        //    gameObject.GetComponent<PlayerManager>().timeSinceLastAttack = 0.0f;
        //    hitEvent.Invoke();
        //}

        //void HitServerRpc()
        //Maybe dont use the animation event as the Hit, but call Hit at a point where the animation plays? 


        void Hit()
        {
            if (target == null)  return;
            if (target.IsDead()) return;


            float damage = currentWeaponConfig.GetDamage();
            damage += GetComponent<BaseStats>().GetStat(Stat.Damage);


            //Critical Hit Code
            if (baseStats.GetStat(Stat.CriticalHitChance) >= UnityEngine.Random.Range(0, 100))
            {
                //Maybe play a UI element that says CRIT!
                Debug.Log("Critical!");
                damage *= criticalDamageBonus;
            }


            if (gameObject.tag == "Enemy")
            {
                //damage = damage + currentWeaponConfig.GetDamage();
                gameObject.GetComponent<AIController>().Aggrevate();
            }

            BaseStats targetBaseStats = target.GetComponent<BaseStats>();
            if (targetBaseStats != null)
            {
                float defence = targetBaseStats.GetStat(Stat.Defence);
                damage /= 1 + defence / damage;
            }

            if (currentWeapon != null)
            {
                currentWeapon.value.OnHit();
            }


            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }

            else
            {
                hitEvent.Invoke();
                target.GetComponent<Health>().DealDamage(gameObject, damage);
            }

        }
        void Shoot()
        {
            Hit();
        }



        //public void SetTarget(GameObject target)
        //{
        //    var targetObject = target.GetComponent<NetworkObject>();
        //    SetTargetServerRpc(targetObject);
        //}

        //[ServerRpc]
        //private void SetTargetServerRpc(NetworkObjectReference networkTarget)
        //{
        //    if (networkTarget.TryGet(out NetworkObject targetObject))
        //    {
        //        target = targetObject.GetComponent<Health>();
        //    }
        //}



        public Health GetTarget()
        {
            return target;
        }
        public Transform GetHandTransform(bool isRightHand)
        {
            if (isRightHand)
            {
                return rightHandTransform;
            }
            else
            {
                return leftHandTransform;
            }
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }
    }

}
