using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;
        [SerializeField] Skill skillToGainExperienceToward;
        [SerializeField] float experienceToGain;
        private void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public CursorType GetCursorType()
        {
            //if (pickup.CanBePickedUp())
            //{
            //    return CursorType.Pickup;
            //}
            //else
            //{
            //    return CursorType.FullPickup;
            //}

            PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
            if (Vector3.Distance(this.transform.position, playerManager.transform.position) <= 2.5f)
            {
                return CursorType.Pickup;
            }

            return CursorType.None;
        }


        public bool HandleRaycast(PlayerManager playerManager)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (Vector3.Distance(playerManager.transform.position, this.transform.position) <= 2.5f)
                {
                    GameObject player = GameObject.FindWithTag("Player");
                    player.GetComponent<Animator>().SetTrigger("gatherAction");
                    player.GetComponent<InputManager>().lootAudioOpen.Play();
                    player.transform.LookAt(this.transform, Vector3.up);
                    player.GetComponent<SkillExperience>().GainExperience(skillToGainExperienceToward, experienceToGain);
                    pickup.PickupItem();

                }

            }
            return true;
        }

    }
}