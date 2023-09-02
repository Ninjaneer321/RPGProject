using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAbilitiesUI : MonoBehaviour
{
    [SerializeField] AbilityUI abilityUI = null;

    public AbilityInventory abilityInventory;
    private void Awake()
    {
        var player = GameObject.FindWithTag("Player");
        abilityInventory = player.GetComponent<AbilityInventory>();
    }

    private void Start()
    {
        abilityInventory.SetupAbilities(abilityInventory.GetAbilitiesBank());
    }

}
