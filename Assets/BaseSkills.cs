using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Stats
{
        public class BaseSkills : MonoBehaviour
        {
        //[Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] SkillProgression skillProgression = null;
        public event Action<Skill> onSkillLevelUp;
        private Dictionary<Skill, SkillExperience> skillExperiences;
        [SerializeField] LazyValue<int> currentLevel;     
        private void OnEnable()
        {
            foreach (var skillExperience in skillExperiences.Values)
            {
                skillExperience.onSkillExperienceGained += UpdateLevel;   
                
            }
        }
        private void OnDisable()
        {
            foreach (var skillExperience in skillExperiences.Values)
            {
                skillExperience.onSkillExperienceGained -= UpdateLevel;
            }
        }

        private void Awake()
        {
            // Cache SkillExperience components
            skillExperiences = new Dictionary<Skill, SkillExperience>();

            SkillExperience skillExperience = GetComponent<SkillExperience>();

            if (skillExperience != null)
            {
                List<Skill> skills = skillExperience.GetSkills();
                foreach (Skill skill in skills)
                {
                    skillExperiences[skill] = skillExperience;
                    currentLevel = new LazyValue<int>(() => CalculateLevel(skill));
                }
            }
            //foreach (var skillExperience in skillExperienceComponents)
            //{
            //    Skill skill = (Skill)skillExperience.GetSkill();
            //    skillExperiences[skill] = skillExperience;
            //    currentLevel = new LazyValue<int>(() => CalculateLevel(skill)); // Initialize LazyValue for each skill  
            //}

            LogDictionary(skillExperiences);
        }

        public CharacterClass GetCharacterClass()
        {
            return characterClass;
        }
        private void LogDictionary(Dictionary<Skill, SkillExperience> dictionary)
        {
            foreach (var pair in dictionary)
            {
                Skill skill = pair.Key;
                SkillExperience skillExperience = pair.Value;
            }
        }
        private void Start()
        {
            //currentLevel = CalculateLevel();
            currentLevel.ForceInit();
        }
        private void UpdateLevel(Skill skill) // Modify to include the skill parameter
        {
            SkillExperience skillExperience = GetSkillExperience(skill);
            int newLevel = CalculateLevel(skill);
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                skillExperience.GainLevel(skill);
                //Set Experience Needed for Next Level in Skill Row UI
                print("Levelled Up!");
                //LevelUpEffect();
                onSkillLevelUp?.Invoke(skill); // Invoke the event passing the skill
            }
        }
        public int GetSkill(Skill skill)
        {
            return GetBaseSkill(skill);
        }


        private int GetBaseSkill(Skill skill)
        {
            return skillProgression.GetSkill(skill, characterClass, GetLevel());
        }
        public int GetLevel()
        {
            return currentLevel.value;
        }
        //CURRENTLY FORCES THE CALCULATION OF THE LEVEL OF THE FORGING SKILL. IDEALLY WE WANT THIS CODE TO WORK FOR ANY AND ALL SKILL I CREATE
        private int CalculateLevel(Skill skill) // Modify to include the skill parameter
        {
            if (!skillExperiences.ContainsKey(skill))
            {
                Debug.LogWarning($"SkillExperience component for skill {skill} is missing. Returning default level.");
                return startingLevel;
            }
            SkillExperience skillExperience = GetSkillExperience(skill);
            float currentXP = skillExperience.GetExperience(skill);
            int penultimateLevel = skillProgression.GetLevels(skill, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float[] XPToLevelUp = skillProgression.GetExperienceToLevel(skill, characterClass);


                if (XPToLevelUp[level -1] > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

        private SkillExperience GetSkillExperience(Skill skill)
        {
            if (skillExperiences.TryGetValue(skill, out SkillExperience skillExperience))
            {
                return skillExperience;
            }
            return null;
        }
    }
}

