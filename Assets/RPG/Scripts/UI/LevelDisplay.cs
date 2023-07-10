using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using Stats;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    BaseStats baseStats;
    Experience experience;
    [SerializeField] TextMeshProUGUI levelValue = null;
    [SerializeField] TextMeshProUGUI experienceValue = null;

    private void Awake()
    {
        baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
    }

    private void Update()
    {
        levelValue.text = baseStats.GetLevel().ToString();

       // experienceValue.text = experience.GetExperience().ToString() + "/" + baseStats.GetStat(Stat.ExperienceToLevelUp).ToString();

        experienceValue.text = string.Format("{0:0}/{1:0}", experience.GetExperience(), baseStats.GetStat(Stat.ExperienceToLevelUp));
    }
}
