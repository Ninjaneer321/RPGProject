using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [SerializeField] GameObject recipePrefab = null;
    [SerializeField] CraftingSlotUI itemSlot = null;
    [SerializeField] GameObject recipeArrow = null;
    [SerializeField] Button craftButton = null;

    [SerializeField] CraftingRecipeBank craftingRecipeBank;
    Inventory inventory;

    private void Awake()
    {
        inventory = Inventory.GetPlayerInventory();
        craftingRecipeBank = Resources.Load<CraftingRecipeBank>("");
    }

    public CraftingRecipeBank GetCraftingRecipeBank()
    {
        return craftingRecipeBank;
    }
    //THIS METHOD IS THE ONE THAT I COULD MAYBE USE TO ADD RECIPES TO MY CRAFTING
    public void SetupCraftingRecipes(CraftingRecipeBank recipe)
    {
        craftingRecipeBank = recipe;
        Redraw();
    }

    private void OnEnable()
    {
        Redraw();
    }

    private void Redraw()
    {
        DestroyChild(transform);

        for (int i = 0; i < craftingRecipeBank.GetCraftingRecipes().Length; i++)
        {
            // Create a recipe holder gameobject in under the current transform.
            var recipeHolder = Instantiate(recipePrefab, transform);
            // Destroy all of the child elements in the recipe holder gameobject.
            DestroyChild(recipeHolder.transform);

            // Assign the current recipe iteration to a new variable.
            var recipe = craftingRecipeBank.GetCraftingRecipes()[i];

            // Create and setup recipe ingredient gameobject elements under the recipe holder gameobject transform.
            CreateRecipeIngredients(recipe, recipeHolder.transform);

            // Create and setup recipe objects. Arrow image, recipe result item and craft button.
            CreateRecipeObjects(craftingRecipeBank.GetCraftingRecipes()[i].item, recipeHolder.transform, recipe);

        }
    }

    private void CreateRecipeIngredients(CollectableRecipe recipe, Transform recipeHolder)
    {
        // Store recipe ingredients length in a variable.
        int ingredientsSize = recipe.ingredients.Length;
        // Loop through all of the ingredients in the recipe.
        for (int ingredient = 0; ingredient < ingredientsSize; ingredient++)
        {
            // Create the itemSlot prefab and make it a child under the recipeHolder transform.
            var ingredientItem = Instantiate(itemSlot, recipeHolder);

            // Set up the item’s (ingredientItem) icon and number amount.
            ingredientItem.Setup(recipe.ingredients[ingredient].item, recipe.ingredients[ingredient].number);
        }
    }

    //private void CreateRecipeIngredients(CraftingRecipeBank.Recipes recipe, Transform recipeHolder)
    //{
    //    // Store recipe ingredients length in a variable.
    //    int ingredientsSize = recipe.ingredients.Length;
    //    // Loop through all of the ingredients in the recipe.
    //    for (int ingredient = 0; ingredient < ingredientsSize; ingredient++)
    //    {
    //        // Create the itemSlot prefab and make it a child under the recipeHolder transform.
    //        var ingredientItem = Instantiate(itemSlot, recipeHolder);

    //        // Set up the item’s (ingredientItem) icon and number amount.
    //        ingredientItem.Setup(recipe.ingredients[ingredient].item, recipe.ingredients[ingredient].number);
    //    }
    //}

    private void CreateRecipeObjects(InventoryItem inventoryItem, Transform recipeHolder, CollectableRecipe recipe)
    {
        // Create the arrow image UI element and make it a child under the recipeHolder transform.
        var arrow = Instantiate(recipeArrow, recipeHolder);

        // Instantiate the itemSlot prefab and make it a child under the recipeHolder transform.
        var item = Instantiate(itemSlot, recipeHolder);
        // Set up the item’s (item) icon and number amount.
        item.Setup(inventoryItem, 1);

        // Create the crafting button UI element and make it a child under the recipeHolder transform.
        var button = Instantiate(craftButton, recipeHolder);
        // Get the Craft script component from the button gameobject and store it in a variable.
        var craft = button.GetComponent<Crafting>();
        // Add a listener to the button onClick event using a lambda expression.
        button.onClick.AddListener(() => craft.CraftItem(inventory, inventoryItem, recipe));
    }

    private void DestroyChild(Transform transform)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
