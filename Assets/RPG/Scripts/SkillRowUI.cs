using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEngine;

public class SkillRowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;

    [SerializeField] BaseSkills baseSkills;
    [SerializeField] SkillExperience skillExperience;

    [SerializeField] Skill skill;

    private void Start()
    {
        baseSkills = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseSkills>();
        skillExperience = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillExperience>();
    }

    private void Update()
    {
        int skillLevel = skillExperience.GetLevel(skill);
        valueText.text = skillLevel.ToString();
    }


}
