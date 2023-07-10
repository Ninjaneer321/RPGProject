using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;
using RPG.Stats;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] public string conversantName;
        [SerializeField] Dialogue newDialogue = null;

        [SerializeField] public bool isActive = false;

        
        public CursorType GetCursorType()
        {
            PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            if (Vector3.Distance(this.transform.position, playerManager.transform.position) <= 2.5f)
            {
                return CursorType.Dialogue;
            }

            return CursorType.NPC;
        }
        public string GetName()
        {
            return conversantName;
        }

        public bool HandleRaycast(PlayerManager playerManager)
        {
            if (newDialogue == null)
            {
                return false;
            }
            if (GetComponent<Health>().IsDead()) return false;
            if (Input.GetMouseButtonDown(1))
            {
                playerManager.GetComponent<Fighter>().target = gameObject.GetComponent<Health>();
                if (Vector3.Distance(this.transform.position, playerManager.transform.position) <= 2.5f)
                {
                    playerManager.GetComponent<PlayerConversant>().StartDialogue(this, newDialogue);
                }
            }
            return true;
        }


    }
}

