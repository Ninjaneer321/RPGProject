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
        //We do not want more than two copies of an Ability on a slot.
        return 1;
    }

    public void AddItems(Ability item, int number)
    {
        //Add using a method created on AbilityInventory.cs (like InventorySlotUI.cs)
        Debug.Log("AddItems in AbilitySlotUI.cs");
    }

    public Ability GetItem()
    {
        return ability;
    }

    public int GetNumber()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveItems(int number)
    {
        Debug.Log("RemoveItems in AbilitySlotUI.cs");
    }
}
