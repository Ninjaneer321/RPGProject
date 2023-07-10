using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using Stats;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, IPredicateEvaluator
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;
        [SerializeField] LazyValue<int> currentLevel;

        Experience experience;


        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            //currentLevel = CalculateLevel();
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }
        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                print("Levelled Up!");
                LevelUpEffect();
                onLevelUp();
            }
        }
        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public int GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private int GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        private int GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            int total = 0;
            foreach (iModifierProvider provider in GetComponents<iModifierProvider>())
            {
                foreach (int modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private int GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            int total = 0;
            foreach (iModifierProvider provider in GetComponents<iModifierProvider>())
            {
                foreach (int modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            if (experience == null) return startingLevel;

            float currentXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);

                Debug.Log(XPToLevelUp);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            if (predicate == EPredicate.MinimumLevel)
            {
                Debug.Log($"Condition is:  if({predicate}({parameters[0]})");
                if (int.TryParse(parameters[0], out int testLevel))
                {
                    return currentLevel.value >= testLevel;
                }
            }
            return null;
        }
    }
}

