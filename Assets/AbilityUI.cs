using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using UnityEngine;

public class AbilityUI : MonoBehaviour
{

    [SerializeField] AbilitySlotUI abilitySlot = null;
    [SerializeField] GameObject abilityRowPrefab = null;

    public AbilityInventory abilityInventory;

    private void Awake()
    {
        abilityInventory = AbilityInventory.GetPlayerAbilityInventory();
        abilityInventory.abilityInventoryUpdated += Redraw;
    }
    private void Start()
    {
        Redraw();
    }

    public void Redraw()
    {
        DestroyChild(transform);

        for (int i = 0; i < abilityInventory.GetAbilitiesBank().GetAbilities().Length; i++)
        {
            var abilityHolder = Instantiate(abilityRowPrefab, transform);
            DestroyChild(abilityHolder.transform);
            var ability = abilityInventory.GetAbilitiesBank().GetAbilities()[i];
            //Create and setup ability objects. 
            CreateAbilityObjects(ability, abilityHolder.transform);
        }
    }

    private void CreateAbilityObjects(Ability ability, Transform abilityHolder)
    {
        var item = Instantiate(abilitySlot, abilityHolder);
        item.Setup(ability);

    }


    private void DestroyChild(Transform transform)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
