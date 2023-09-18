using System;
using System.Collections;
using System.Collections.Generic;
using Stats;
using UnityEngine;


namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]

    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, int[]>> lookupTable = null;

       
        public int GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            int[] levels = lookupTable[characterClass][stat];

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
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            int[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }


        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, int[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, int[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] public CharacterClass characterClass;

            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public int[] levels;
        }
    }
}
