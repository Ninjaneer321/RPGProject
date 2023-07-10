using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCraftingUI : MonoBehaviour
{
    [SerializeField] CraftingRecipe craftingRecipe = null;
    [SerializeField] CraftingUI craftingItems = null;
    [SerializeField] GameObject craftingUI = null;


    private void Awake()
    {

    }

    private void Start()
    {

        //craftingUI.SetActive(true);

        craftingItems.SetupCraftingRecipes(craftingRecipe);
    }


}

