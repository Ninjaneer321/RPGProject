using System.Collections;
using System.Collections.Generic;
using Stats;
using UnityEngine;


namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "SkillProgression", menuName = "Stats/New Skill Progression", order = 0)]
    public class SkillProgression : ScriptableObject
    {
        [SerializeField] ProgressionSkillClass[] skillClasses = null;

        Dictionary<CharacterClass, Dictionary<Skill, (int[], float[])>> lookupTable = null;

        public int GetLevels(Skill skill, CharacterClass characterClass)
        {
            BuildLookup();

            (int[], float[]) levelData = lookupTable[characterClass][skill];
            //int[] levels = lookupTable[characterClass][stat];
            int[] levels = levelData.Item1;

            return levels.Length;

        }

        public float[] GetExperienceToLevel(Skill skill, CharacterClass characterClass)
        {
            BuildLookup();

            (int[], float[]) levelData = lookupTable[characterClass][skill];

            float[] expToNextLevel = levelData.Item2;

            return expToNextLevel;
        }

        public int GetSkill(Skill skill, CharacterClass characterClass, int level)
        {
            BuildLookup();

            (int[], float[]) skillData = lookupTable[characterClass][skill];

            int[] levels = skillData.Item1;

            if (levels.Length == 0)
            {
                return 0;
            }

            if (levels.Length < level)
            {
                return levels[levels.Length - 1];
            }

            return levels[level - 1];
        }
        private void BuildLookup()
        {

            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Skill, (int[], float[])>>();

            foreach (ProgressionSkillClass progressionClass in skillClasses)
            {
                var skillLookupTable = new Dictionary<Skill, (int[], float[])>();

                foreach (ProgressionSkill progressionSkill in progressionClass.skills)
                {
                    skillLookupTable[progressionSkill.skill] = (progressionSkill.levels, progressionSkill.experienceToLevel);
                }
                lookupTable[progressionClass.characterClass] = skillLookupTable;
            }
        }

    }


    [System.Serializable]
    class ProgressionSkillClass
    {
        [SerializeField] public CharacterClass characterClass;

        public ProgressionSkill[] skills;
    }

    [System.Serializable]
    class ProgressionSkill
    {
        [SerializeField] public Skill skill;
        public int[] levels;
        public float[] experienceToLevel;
    }

}

