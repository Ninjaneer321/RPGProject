using System.Collections;
using System.Collections.Generic;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Core;
using UnityEngine;

public class AchievementCounter : MonoBehaviour, ISaveable , IPredicateEvaluator
{
    public event System.Action onCountChanged;

    private Dictionary<string, int> counts = new Dictionary<string, int>();
    //Creates and initializes a Dictionary with string Keys and int Values.

    public int AddToCount(string token, int amount, bool onlyIfExists = false)
    {
        if (!counts.ContainsKey(token))
        {
            if (onlyIfExists)
            {
                return 0;
            }
            counts[token] = amount;
            onCountChanged?.Invoke();
            return amount;
        }
        counts[token] += amount;
        onCountChanged?.Invoke();
        return counts[token];
    }

    public int RegisterCounter(string token, bool overwrite = false)
    {
        if (!counts.ContainsKey(token) || overwrite)
        {
            counts[token] = 0;
            onCountChanged?.Invoke();
        }
        return counts[token];
    }

    public int GetCounterValue(string token)
    {
        if (!counts.ContainsKey(token)) return 0;
        return counts[token];
    }

    public object CaptureState()
    {
        return counts;
    }

    public void RestoreState(object state)
    {
        counts = (Dictionary<string, int>)state;
        onCountChanged?.Invoke();
    }

    public bool? Evaluate(EPredicate predicate, string[] parameters)
    {
        if (predicate == EPredicate.HasKilled)
        {
            Debug.Log($"Condition is:  if({predicate}({parameters[0]}, {parameters[1]})");
            if (int.TryParse(parameters[1], out int intParameter))
            {
                RegisterCounter(parameters[0]);
                Debug.Log(counts[parameters[0]] >= intParameter);
                return counts[parameters[0]] >= intParameter;
            }
            Debug.Log($"Parameters[1] ({parameters[1]}) is not an integer.");
            return false;
        }
        return null;
    }
}