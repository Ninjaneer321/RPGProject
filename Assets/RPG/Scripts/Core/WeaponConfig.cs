using RPG.Stats;
using UnityEngine;
using System.Collections.Generic;
using GameDevTV.Inventories;
using Stats;
using RPG.Inventories;

namespace RPG.Core
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Equipment/Weapon/Make New Weapon", order = 0)]
    public class WeaponConfig : StatsEquipableItem, iModifierProvider
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] Weapon weaponPrefab2 = null;
        [Header("Add +Damage Stat and +1 to Top and Bottom except Unarmed")]
        [SerializeField] public int weaponDamageTopEnd;
        [SerializeField] public int weaponDamageBottomEnd;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] public float weaponRange;
        [SerializeField] public float weaponSpeed;
        [SerializeField] Projectile projectile = null;

        [SerializeField] public Skill weaponSkill;
        const string weaponName = "Weapon";
        const string weaponName2 = "Weapon2";

        public bool isTwoHanded = false;
        public enum WeaponHands
        {
            right,
            left,
            both
        }
        [SerializeField] WeaponHands mainHand;
        private bool isDualWield;

        private void Awake()
        {
        }

        public float GetRange()
        {
            return weaponRange;
        }
        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        private float GetTopEnd()
        {
            return weaponDamageTopEnd;
        }

        private float GetBottomEnd()
        {
            return weaponDamageBottomEnd;
        }
        public float GetDamage()
        {
            float randomDamage = Random.Range(GetBottomEnd(), GetTopEnd());
            return randomDamage;
            //return weaponDamageTopEnd;
        }
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {

            DestroyOldWeapon(rightHand, leftHand);
            DestroyOtherWeapon(rightHand, leftHand);

            Weapon weapon = null;
            Weapon weapon2 = null;

            if (weaponPrefab != null)
            {
                Transform handTransform = GetMainHand(rightHand, leftHand);

                if (mainHand == WeaponHands.left)
                {
                    weapon = Instantiate(weaponPrefab, handTransform);
                    weapon.gameObject.name = weaponName;
                }
                if (mainHand == WeaponHands.right)
                {
                    weapon = Instantiate(weaponPrefab, handTransform);
                    weapon.gameObject.name = weaponName;

                }
                if (mainHand == WeaponHands.both)
                {
                    weapon = Instantiate(weaponPrefab, rightHand);
                    weapon.gameObject.name = weaponName;
                    if (weaponPrefab2 != null && isDualWield)
                    {
                        weapon2 = Instantiate(weaponPrefab2, leftHand);
                        weapon2.gameObject.name = weaponName2;
                    }
                }
                if (animator.runtimeAnimatorController != null)
                {
                    animator.runtimeAnimatorController = animatorOverride;
                }
            }
            return weapon;
        }

        private void DestroyOtherWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon2 = leftHand.Find(weaponName2);

            if (oldWeapon2 == null)
            {
                oldWeapon2 = rightHand.Find(weaponName2);
            }

            if (oldWeapon2 == null) return;

            oldWeapon2.name = "DESTROYING";

            Destroy(oldWeapon2.gameObject);
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
        private Transform GetMainHand(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (mainHand == WeaponHands.right) return rightHand;
            if (mainHand == WeaponHands.left) return leftHand;
            if (mainHand == WeaponHands.both)
            {
                if (rightHand && !leftHand)
                {
                    return rightHand;
                }
                if (leftHand && !rightHand)
                {
                    return leftHand;
                }
            }
            return handTransform = rightHand;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetMainHand(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

    }
}