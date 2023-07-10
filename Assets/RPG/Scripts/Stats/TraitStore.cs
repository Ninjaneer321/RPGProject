using System;
using System.Collections;
using GameDevTV.Saving;
using System.Collections.Generic;
using Stats;
using UnityEngine;
using GameDevTV.Utils;
using System.Linq;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, iModifierProvider, ISaveable, IPredicateEvaluator
    {


        [System.Serializable]
        class TraitBonus
        {
            public Stat traitStat; //TraitStat
            public Stat stat; //Stat

            public int perLevel = 1;

        }

        [SerializeField] private List<TraitBonus> bonuses = new List<TraitBonus>();

        public event Action onStatsChanged;

        private BaseStats stats;

        Dictionary<Stat, int> assignedPoints = new Dictionary<Stat, int>();
        Dictionary<Stat, int> stagedPoints = new Dictionary<Stat, int>();

        private Dictionary<Stat, Dictionary<Stat, int>> modifiers =
            new Dictionary<Stat, Dictionary<Stat, int>>();

        private void Awake()
        {
            stats = GetComponent<BaseStats>();

            foreach (TraitBonus bonus in bonuses)
            {
                if (!modifiers.ContainsKey(bonus.stat))
                {
                    modifiers[bonus.stat] = new Dictionary<Stat, int>();
                }

                modifiers[bonus.stat][bonus.traitStat] = bonus.perLevel;
            }
        }

        public int GetProposedPoints(Stat stat)
        {
            return GetPoints(stat) + GetStagedPoints(stat);
        }

        public int GetPoints(Stat stat)
        {
            return assignedPoints.ContainsKey(stat) ? assignedPoints[stat] : 0;
        }

        public int GetStagedPoints(Stat stat)
        {
            return stagedPoints.ContainsKey(stat) ? stagedPoints[stat] : 0;
        }

        public void AssignPoints(Stat stat, int amount)
        {
            if (!CanAssignPoints(stat, amount)) return;

            stagedPoints[stat] = GetStagedPoints(stat) + amount;
        }
            
        public bool CanAssignPoints(Stat stat, int points)
        {
            if (GetStagedPoints(stat) + points < 0) return false;
            if (GetUnallocatedPoints() < points) return false;
            return true;
        }

        public int GetUnallocatedPoints()
        {
            return GetAssignablePoints() - GetTotalProposePoints();
        }

        public int GetTotalProposePoints()
        {
            int total = 0;

            foreach (int points in assignedPoints.Values)
            {
                total += points;
            }

            foreach (int points in stagedPoints.Values)
            {
                total += points;
            }

            return total;

        }

        public void Commit()
        {
            foreach (Stat stat in stagedPoints.Keys)
            {
                Debug.Log(GetProposedPoints(stat));
                assignedPoints[stat] = GetProposedPoints(stat);
            }
            stagedPoints.Clear();
        }

        public int GetTrait(Stat stat)
        {
            if (!stats) stats = GetComponent<BaseStats>();
            return (assignedPoints.ContainsKey(stat) ? assignedPoints[stat] : 0) + stats.GetStat(stat);
        }

        public int GetAssignablePoints()
        {
            return stats.GetStat(Stat.TotalTraitPoints);
        }

        public IEnumerable<int> GetAdditiveModifiers(Stat stat)
        {
            if (modifiers.ContainsKey(stat))
            {
                foreach (KeyValuePair<Stat, int> pair in modifiers[stat])
                {
                    yield return GetTrait(pair.Key) * pair.Value;
                }
            }
        }

        public IEnumerable<int> GetPercentageModifiers(Stat stat)
        {
            yield break;
        }


        public object CaptureState()
        {
            return assignedPoints;
        }

        public void RestoreState(object state)
        {
            assignedPoints = new Dictionary<Stat, int>((IDictionary<Stat, int>)state);
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
           if (predicate == EPredicate.MinimumTrait)
             {
                if (Enum.TryParse<Stat>(parameters[0], out Stat trait))
                {
                    Debug.Log($"Para[0]:{parameters[0]}/Pts:{GetTrait(trait)} | Para[1] Points:{parameters[1]}\nIsPara[0] >= Para[1]  ? {GetTrait(trait) >= Int32.Parse(parameters[1])}");

                    return GetTrait(trait) >= Int32.Parse(parameters[1]);
                }
                return false;
            }
            return null;
        }
    }
}
