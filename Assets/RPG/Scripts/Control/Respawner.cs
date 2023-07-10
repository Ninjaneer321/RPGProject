using System;
using System.Collections;
using RPG.SceneManagement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] Transform respawnLocation;
        [SerializeField] float respawnDelay = 3;
        [SerializeField] float fadeTime = 2.0f;
        [SerializeField] float healthRegenPercentage = 20;

        Animator animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
            GetComponent<Health>().onDie.AddListener(Respawn);
        }

        public void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeTime);
            animator.SetBool("dead", false);
            transform.position = respawnLocation.position;
            //GetComponent<NavMeshAgent>().Warp(respawnLocation.position);
            Health health = GetComponent<Health>();
            health.Heal(health.GetMaxHealthPoints() * healthRegenPercentage / 100);
            health.isDead = false;
            yield return fader.FadeIn(fadeTime);
        }
    }
}