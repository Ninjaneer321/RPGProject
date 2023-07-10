using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using RPG.Stats;
using UnityEngine;

public class SkillExperience : MonoBehaviour, ISaveable
{
    [Serializable]
    public class SkillExperienceData
    {
        public Skill skill;
        public float experience;
    }

    [SerializeField] private List<SkillExperienceData> skillExperienceDataList = new List<SkillExperienceData>();

    public event Action<Skill> onSkillExperienceGained;

    public float GetExperience(Skill skill)
    {
        SkillExperienceData skillData = GetSkillData(skill);
        return skillData != null ? skillData.experience : 0f;
    }
    public Skill? GetSkill()
    {
        if (skillExperienceDataList.Count > 0)
        {
            return skillExperienceDataList[0].skill;
        }
        return null;
    }
    public void GainExperience(Skill skill, float experience)
    {
        SkillExperienceData skillData = GetOrCreateSkillData(skill);
        skillData.experience += experience;
        onSkillExperienceGained?.Invoke(skill);
    }

    private SkillExperienceData GetSkillData(Skill skill)
    {
        return skillExperienceDataList.Find(data => data.skill == skill);
    }

    private SkillExperienceData GetOrCreateSkillData(Skill skill)
    {
        SkillExperienceData skillData = GetSkillData(skill);
        if (skillData == null)
        {
            skillData = new SkillExperienceData { skill = skill };
            skillExperienceDataList.Add(skillData);
        }
        return skillData;
    }

    public object CaptureState()
    {
        return skillExperienceDataList;
    }

    public void RestoreState(object state)
    {
        skillExperienceDataList = (List<SkillExperienceData>)state;
    }
}
