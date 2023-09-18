using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillRowUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelValueText;
    [SerializeField] TextMeshProUGUI experienceLabel;
    [SerializeField] Slider slider;
    [SerializeField] BaseSkills baseSkills;
    [SerializeField] SkillExperience skillExperience;
    [SerializeField] Skill skill;
    [SerializeField] SkillProgression skillProgression;
    [SerializeField] float normalizedExperienceValue; 
    [SerializeField] float experienceAmountNeededToLevelUp;
    [SerializeField] float experienceNeededBetweenLevels;
    [SerializeField] float excessExperience;

    private void Awake()
    {
        baseSkills = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseSkills>();
        skillExperience = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillExperience>();

    }
    private void Start()
    {
        experienceAmountNeededToLevelUp = SkillExperienceToNextLevel()[SkillLevel(skill)];
        baseSkills.onSkillLevelUp += SetExperienceAmountNeededToLevelUp;

    }

    //set up the slider value to be a normalized value of my skill experience / the skill experience to next level.

    private void Update()
    {
        experienceNeededBetweenLevels = SkillExperienceToNextLevel()[SkillLevel(skill) + 1] - SkillExperienceToNextLevel()[SkillLevel(skill)];
        levelValueText.text = (SkillLevel(skill) +1).ToString();
        normalizedExperienceValue = SkillExperienceGainedTowardsLevel() / experienceNeededBetweenLevels;
        slider.value = normalizedExperienceValue;
        experienceLabel.text = SkillExperienceGainedTowardsLevel().ToString() + " / " + experienceNeededBetweenLevels.ToString();
    }

    public float GetExperienceNeededBetweenLevels()
    {
        return experienceNeededBetweenLevels;
    }
    public float SkillExperience()
    {
        return skillExperience.GetExperience(skill);
    }
    public int SkillLevel(Skill skill)
    {
        return skillExperience.GetLevel(skill);
    }

    public float SkillExperienceGainedTowardsLevel()
    {
        return skillExperience.GetExperienceTowardsNextLevel(skill);
    }
    public float[] SkillExperienceToNextLevel()
    {
        return skillProgression.GetExperienceToLevel(skill, baseSkills.GetCharacterClass());
    }
    public void SetExperienceAmountNeededToLevelUp(Skill context)
    {
        experienceAmountNeededToLevelUp = SkillExperienceToNextLevel()[SkillLevel(skill) + 1];

        excessExperience = SkillExperience() - SkillExperienceToNextLevel()[SkillLevel(skill)];

        skillExperience.SetExcessExperienceGainedTowardsNextLevel(excessExperience, skill);
        Debug.Log(excessExperience);
    }



}
