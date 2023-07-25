using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillRowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Slider slider;
    [SerializeField] BaseSkills baseSkills;
    [SerializeField] SkillExperience skillExperience;

    [SerializeField] Skill skill;

    [SerializeField] SkillProgression skillProgression;

    private void Start()
    {
        baseSkills = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseSkills>();
        skillExperience = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillExperience>();
    }

    //set up the slider value to be a normalized value of my skill experience / the skill experience to next level.

    private void Update()
    {
        int skillLevel = skillExperience.GetLevel(skill);
        valueText.text = skillLevel.ToString();

        Debug.Log(SkillExperience());
        Debug.Log(SkillExperienceToNextLevel()[skillLevel + 1]);
    }

    private float SkillExperience()
    {
        return skillExperience.GetExperience(skill);
    }

    private float[] SkillExperienceToNextLevel()
    {
        return skillProgression.GetExperienceToLevel(skill, baseSkills.GetCharacterClass());
    }


}
