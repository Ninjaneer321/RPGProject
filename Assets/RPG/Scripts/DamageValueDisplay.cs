using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using Stats;
using TMPro;
using UnityEngine;

public class DamageValueDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageValue = null;

    BaseStats baseStats;
    Fighter fighter;

    float bottomEnd;
    float topEnd;
    // Start is called before the first frame update
    void Start()
    {
        baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();

        fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDamage();

        damageValue.text = bottomEnd.ToString() + " - " + topEnd.ToString();
    }


    void GetDamage()
    {
        topEnd = fighter.currentWeaponConfig.weaponDamageTopEnd + (baseStats.GetStat(Stat.Damage));
        bottomEnd = fighter.currentWeaponConfig.weaponDamageBottomEnd + (baseStats.GetStat(Stat.Damage));
    }
}
