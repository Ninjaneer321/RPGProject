using System.Collections;
using System.Collections.Generic;
using RPG.Abilities;
using UnityEngine;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] AbilitiesBank abilitiesBank;
    [SerializeField] AbilitySlotUI abilitySlot = null;
    [SerializeField] GameObject abilityRowPrefab = null;

    public AbilitiesBank GetAbilitiesBank()
    {
        return abilitiesBank;
    }
    public void SetupAbilities(AbilitiesBank bank)
    {
        abilitiesBank = bank;
        Redraw();
    }

    public void Redraw()
    {
        DestroyChild(transform);

        for (int i = 0; i < abilitiesBank.GetAbilities().Length; i++)
        {
            var abilityHolder = Instantiate(abilityRowPrefab, transform);
            DestroyChild(abilityHolder.transform);
            var ability = abilitiesBank.GetAbilities()[i];
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
