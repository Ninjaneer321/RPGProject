using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Combat;
using GameDevTV.Saving;
using RPG.Stats;
using Stats;
using GameDevTV.Utils;
using RPG.Movement;
using System;
using RPG.Dialogue;
using UnityEngine.AI;
using GameDevTV.Inventories;
using RPG.Abilities;
using Unity.Netcode;
using UnityEngine.Events;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        // Start is called before the first frame update

              
        [SerializeField] private float chaseDistance = 10f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [SerializeField] float agroCooldownTime = 5f;

        int currentWaypointIndex = 0;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        [SerializeField] public float timeSinceAggrevated = Mathf.Infinity;
        [SerializeField] float timeSinceLastSawPlayer = Mathf.Infinity;

        [SerializeField] GameObject player;
        Health health;
        Mover mover;
        Fighter fighter;
        ActionStore actionStore;
        LazyValue<Vector3> guardPosition;

        public Shader defaultShader;
        public Shader selectionShader;
        public Renderer rend;


        void Awake()
        {

            player = GameObject.FindWithTag("Player");
            health = this.GetComponent<Health>();
            mover = this.GetComponent<Mover>();
            fighter = this.GetComponent<Fighter>();
            actionStore = this.GetComponent<ActionStore>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
            guardPosition.ForceInit();

            defaultShader = Shader.Find("Universal Render Pipeline/Lit");

            
            //selectionShader = Shader.Find("Universal Render Pipeline/Lit");
        }

        private void Start()
        {

            if (patrolPath == null)
            {
                patrolPath = transform.parent.GetComponentInChildren<PatrolPath>();
            }

        }


        // Update is called once per frame
        void Update()
        {
            if (health.IsDead())
            {
                fighter.Cancel();
                //this.GetComponent<CapsuleCollider>().enabled = false;
                this.enabled = false;
                player = null;
                return;
            }

            if (player == null) return;

            if (IsAggrevated())
            {
                AttackBehavior();
                return;
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();

            if (!GetComponent<AIConversant>())
            {
                return;
            }
            else if (GetComponent<AIConversant>().isActive)
            {
                SuspicionBehavior();
                return;
            }


        }

        public void Reset()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(guardPosition.value);
            timeSinceLastSawPlayer = Mathf.Infinity;
            timeSinceArrivedAtWaypoint = Mathf.Infinity;
            timeSinceAggrevated = Mathf.Infinity;
            currentWaypointIndex = 0;
        }
        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private void SuspicionBehavior()
        {
            mover.Stop();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }


        private void PatrolBehavior()
        {
            if (health.IsDead()) return;

            if (health.GetHealthPoints() < health.GetMaxHealthPoints())
            {
                health.ResetHealth();
            }

            fighter.Cancel();
            Vector3 nextPosition = guardPosition.value;
            mover.Walk();
            fighter.isAggrevated = false;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.MoveTo(nextPosition);
            }

        }

        private void AttackBehavior()
        {
            mover.Run();
            timeSinceLastSawPlayer = 0;
            AbilityBehavior();
            fighter.Attack(player);
            fighter.GetTarget().GetComponent<Fighter>().isInCombat = true;
        }

        private void AbilityBehavior()
        {
            if (actionStore != null)
            {
                actionStore.EnemyUse(0, gameObject);
            }

            return;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {   
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }
        
        private bool IsAggrevated()
        {
            if (GetComponent<CombatTarget>() == null) return false;
            if (!GetComponent<CombatTarget>().enabled) return false;
           float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggrevated < agroCooldownTime;        
        }
        public void Cancel()
        {
            mover.Stop();
            player = null;
        }
        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        public void Selected()
        {
            if (rend != null)
            {
                rend.material.shader = selectionShader;
            }           
        }

        public void Deselected()
        {
            if (rend != null)
            {
                rend.material.shader = defaultShader;
            }
            
        }
    }
}

