using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.UI.Inventories;
using UnityEngine;
using RPG.Abilities;
using GameDevTV.Core.UI.Dragging;

public class AbilitySlotUI : MonoBehaviour, IAbilityHolder, IDragContainer<Ability>
{
    [SerializeField] AbilityItemIcon icon = null;
    public Ability ability;


    //STATE


    public void Setup(Ability ability)
    {
        this.ability = ability;
        icon.SetAbility(ability);
    }

    public Ability GetAbility()
    {
        return ability;
    }

    public int MaxAcceptable(Ability item)
    {
        return int.MaxValue;
    }

    public void AddItems(Ability item, int number)
    {
        //Add using a method created on AbilityInventory.cs (like InventorySlotUI.cs)
    }

    public Ability GetItem()
    {
        return ability;
    }

    public int GetNumber()
    {
        return 1;
    }

    public void RemoveItems(int number)
    {

    }
}
