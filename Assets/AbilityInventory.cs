using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using UnityEngine;

public class AbilityInventory : MonoBehaviour
{

    //CONFIG DATA
    [SerializeField] public int abilitiesInventorySize;
    [SerializeField] AbilitiesBank abilitiesBank;

    //STATE
    public AbilitySlot[] slots;

    public struct AbilitySlot
    {
        public Ability ability;
        public int number;
    }
    private void Awake()
    {
        abilitiesInventorySize = abilitiesBank.GetAbilities().Length;
        slots = new AbilitySlot[abilitiesInventorySize];
    }
    public event Action abilityInventoryUpdated;

    public static AbilityInventory GetPlayerAbilityInventory()
    {
        var player = GameObject.FindWithTag("Player");
        return player.GetComponent<AbilityInventory>();
    }
    public AbilitiesBank GetAbilitiesBank()
    {
        return abilitiesBank;
    }
    public void SetupAbilities(AbilitiesBank bank)
    {
        abilitiesBank = bank;
        if (abilityInventoryUpdated != null)
        {
            abilityInventoryUpdated();
        }
    }
}
