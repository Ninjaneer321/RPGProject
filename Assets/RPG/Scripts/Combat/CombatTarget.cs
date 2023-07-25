using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.Control;

namespace RPG.Combat
{

    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerManager playerManager)
        {
            if (!enabled) return false;

            if (Input.GetMouseButtonDown(1))
            {


                if (playerManager.GetComponent<Fighter>().target != null && playerManager.GetComponent<Fighter>().target.tag == "Enemy")
                {
                    playerManager.GetComponent<Fighter>().target.GetComponent<AIController>().Deselected();
                }
                playerManager.GetComponent<Fighter>().target = gameObject.GetComponent<Health>();

                if (playerManager.GetComponent<Fighter>().target != null
                    && playerManager.GetComponent<Fighter>().target.tag == "Enemy")
                {
                    playerManager.GetComponent<Fighter>().target.GetComponent<AIController>().Selected();
                }
                playerManager.isAttacking = true;
                //playerManager.GetComponent<Fighter>().HitTestServerRpc();
                playerManager.AttackTestMethod();
            }
            return true;
        }

    }
}

