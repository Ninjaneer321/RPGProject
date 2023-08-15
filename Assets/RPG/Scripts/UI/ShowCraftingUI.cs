using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCraftingUI : MonoBehaviour
{
    [SerializeField] CraftingRecipeBank craftingRecipe = null;
    [SerializeField] CraftingUI craftingItems = null;


    private void Awake()
    {

    }

    private void Start()
    {
        craftingItems.SetupCraftingRecipes(craftingRecipe);
    }


}

