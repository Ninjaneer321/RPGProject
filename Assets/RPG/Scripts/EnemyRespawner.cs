using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    public bool Death;
    public float Timer;
    public float Cooldown;
    [SerializeField] private GameObject Enemy;
    public string EnemyName;
    [SerializeField] private GameObject LastEnemy;


    void Start()
    {
        Timer = Cooldown + 1f;
        Death = false;
        this.gameObject.name = EnemyName + " spawn point";
    }


    // Update is called once per frame
    void Update()
    {

        if (Death == true)
        {
            //If my enemy is death, a timer will start.
            Timer += Time.deltaTime;

        }
        //If the timer is bigger than cooldown.
        if (Timer >= Cooldown)
        {
            //It will create a new Enemy of the same class, at this position.
            Enemy.transform.position = transform.position;

            var spawnedEnemy = Instantiate(Enemy);
            spawnedEnemy.transform.SetParent(transform);

            LastEnemy = GameObject.Find(Enemy.name + "(Clone)");
            LastEnemy.name = EnemyName;
            //My enemy won't be dead anymore.
            Death = false;
            //Timer will restart.
            Timer = 0;
        }
    }
}
