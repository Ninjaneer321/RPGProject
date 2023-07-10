using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject[] destroyOnHit = null;
    [SerializeField] bool isHoming = true;

    Health target = null;
    GameObject instigator = null;
    public GameObject hitPrefab;

    [SerializeField] UnityEvent onHit;

    float damage = 0;


    [SerializeField] float maxLifeTime = 10f;
    [SerializeField] float lifeAfterImpact = 2f;



    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(GetAimLocation());
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        if (isHoming)
        {
            if (!target.GetComponent<Health>().IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

    }

    public void SetTarget(Health target, GameObject instigator, float damage)
    {
        this.target = target;
        this.instigator = instigator;
        this.damage = damage;

        Destroy(gameObject, maxLifeTime);
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if (targetCapsule == null)
        {
            return target.transform.position;
        }
        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

   
    private void OnTriggerEnter(Collider other)
    {

        Health health = other.GetComponent<Health>();
        if (target != null && health != target) return;
        if (health == null || health.IsDead()) return;

        speed = 0;

        onHit.Invoke();

        Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.forward);
        Vector3 pos = other.transform.position;


        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }


        if (health != null)
        {
            health.DealDamage(instigator, damage);

        }
        foreach (GameObject toDestroy in destroyOnHit)
        {
            Destroy(toDestroy);
        }
        Destroy(gameObject, lifeAfterImpact);
    }
}
