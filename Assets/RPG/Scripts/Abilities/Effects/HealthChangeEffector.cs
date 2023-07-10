using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

public class HealthChangeEffector : MonoBehaviour
{
    private float healthChange;
    private float timeBetweenIntervals;
    private float repeats;
    private GameObject applier;
    private bool started = false;

    private Health health;
    private float timeSinceLastApplied = Mathf.Infinity;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void Apply(GameObject applier, float healthChange, float timeBetweenIntervals, float repeats)
    {
        this.applier = applier;
        this.healthChange = healthChange;
        this.timeBetweenIntervals = timeBetweenIntervals;
        this.repeats = repeats;
        started = true;
    }

    private void Update()
    {
        if (!started) return;
        if (repeats <= 0)
        {
            Destroy(this);
        }

        timeSinceLastApplied += Time.deltaTime;
        if (timeBetweenIntervals > timeSinceLastApplied) return;
        health.DealDamage(applier, healthChange);
        timeSinceLastApplied = 0;
        repeats--;

    }
}
