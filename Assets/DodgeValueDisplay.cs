using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using Stats;
using TMPro;
using UnityEngine;

public class DodgeValueDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dodgeChanceValue = null;

    BaseStats baseStats;

    // Start is called before the first frame update
    void Start()
    {
        baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
    }

    // Update is called once per frame
    void Update()
    {
        dodgeChanceValue.text = baseStats.GetStat(Stat.DodgeChance).ToString() + " %";
    }
}
