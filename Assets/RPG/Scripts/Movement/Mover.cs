using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, ISaveable
    {

        NavMeshAgent navMeshAgent;
        Health health;
        [SerializeField] float walkSpeed;
        [SerializeField] float runSpeed;
        [SerializeField] float maxNavPathLength = 40f;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        //public bool CanMoveTo(Vector3 destination)
        //{
        //    NavMeshPath path = new NavMeshPath();
        //    bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
        //    if (!hasPath) return false;
        //    if (path.status != NavMeshPathStatus.PathComplete) return false;
        //    if (GetPathLength(path) > maxNavPathLength) return false;

        //    return true;
        //}
        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void Stop()
        {
            navMeshAgent.isStopped = true;
        }

        public void Walk()
        {
            navMeshAgent.speed = walkSpeed;
        }

        public void Run()
        {
            navMeshAgent.speed = runSpeed;
        }
        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("speed", speed);

        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}