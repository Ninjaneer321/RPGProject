using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using Stats;
using UnityEngine;

namespace RPG.Core
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Equipment/Armor/Make New Armor", order = 0)]
    public class ShieldConfig : SyntyEquipableItem
    {
        [SerializeField] Shield equippedPrefab;
        [SerializeField] bool isRightHanded;

        const string shieldName = "Shield";
        public Shield SpawnEquipableItem(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldEquipableItem(rightHand, leftHand);

            Shield shield = null;

            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                shield = Instantiate(equippedPrefab, handTransform);
                shield.gameObject.name = shieldName;
            }
            return shield;
        }

        public Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        void DestroyOldEquipableItem(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(shieldName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(shieldName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
    }
}

