using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Inventory/Crafting Recipe Bank"))]
public class CraftingRecipeBank : ScriptableObject
{

	[SerializeField] List<CraftingRecipe> collectableRecipes = new List<CraftingRecipe>();

	public CraftingRecipe[] GetCraftingRecipes()
	{
		return collectableRecipes.ToArray();
	}

	public void AddNewCraftingRecipes(CraftingRecipe newCraftingRecipe)
    {
		Debug.Log("Remember that this function adds to the Scriptable Object which is perma changed until deleted via the Inspector.");
		collectableRecipes.Add(newCraftingRecipe);
		//Redraw CraftingUI 
    }
}
