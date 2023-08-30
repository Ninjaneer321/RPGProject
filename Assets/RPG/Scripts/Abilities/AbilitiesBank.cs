using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Abilities;

[CreateAssetMenu(menuName = ("RPG/Abilities/Ability Recipe Bank"))]
public class AbilitiesBank : ScriptableObject
{
	[SerializeField] List<Ability> abilityList = new List<Ability>();

	public Ability[] GetAbilities()
    {
		return abilityList.ToArray();
    }

	public void AddNewAbility(Ability newAbility)
    {
		abilityList.Add(newAbility);
		//Redraw Ability UI
    }

}
