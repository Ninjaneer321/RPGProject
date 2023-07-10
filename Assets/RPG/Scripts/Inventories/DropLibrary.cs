using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

[CreateAssetMenu(menuName =("RPG/Inventory/Drop Library"))]
public class DropLibrary : ScriptableObject
{
    //Drop Chance
    //Min Drops
    //Max Drops
    //Potential Drops
    //  Relative Chance
    //  Min Items
    //  Max Items
    [SerializeField]
    public DropConfig[] potentialDrops;
    [SerializeField] float[] dropChancePercentage;
    [SerializeField] int[] minNumberOfDrops;
    [Tooltip("Imagine the number you put in Element is minus 1.")]
    [SerializeField] int[] maxNumberOfDrops; 

    [System.Serializable]
    public class DropConfig
    {
        public InventoryItem item;
        public float[] relativeChance;
        public int[] minNumberOfItems;
        public int[] maxNumberOfItems;

        public int GetRandomNumber(int level)
        {
            if (!item.IsStackable())
            {
                return 1;
            }
            int min = GetByLevel(minNumberOfItems, level);
            int max = GetByLevel(maxNumberOfItems, level);
            return UnityEngine.Random.Range(min, max + 1);
        }

    }
    static T GetByLevel<T>(T[] values, int level)
    {
        if (values.Length == 0)
        {
            return default;
        }
        if (level > values.Length)
        {
            return values[values.Length - 1];
        }
        if (level <= 0)
        {
            return default;
        }
        return values[level - 1];
    }
    public struct Dropped
    {
        public InventoryItem item;
        public int number;
    }
    public IEnumerable<Dropped> GetRandomDrops(int level)
    {
        if (!ShouldRandomDrop(level))
        {
            yield break;
        }

        for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
        {
            yield return GetRandomDrop(level);
        }
    }



    private int GetRandomNumberOfDrops(int level)
    {
        int min = GetByLevel(minNumberOfDrops, level);
        int max = GetByLevel(maxNumberOfDrops, level);

        return UnityEngine.Random.Range(min, max);
    }

    private bool ShouldRandomDrop(int level)
    {
        return UnityEngine.Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
    }

    Dropped GetRandomDrop(int level)
    {
        var drop = SelectRandomItem(level);
        var result = new Dropped();
        result.item = drop.item;
        result.number = drop.GetRandomNumber(level);
        return result;
    }



    //By Level SelectRandomItem
    DropConfig SelectRandomItem(int level)
    {
        float totalChance = GetTotalChance(level);
        float randomRoll = UnityEngine.Random.Range(0, totalChance);
        float chanceTotal = 0;
        foreach (var drop in potentialDrops)
        {
            chanceTotal += GetByLevel(drop.relativeChance, level);
            if (chanceTotal > randomRoll)
            {
                return drop;
            }
        }
        return null;
    }

     float GetTotalChance(int level)
    {
        float total = 0;
        foreach (var drop in potentialDrops)
        {
            total += GetByLevel(drop.relativeChance, level);
        }
        return total;
    }
}
