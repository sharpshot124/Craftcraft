using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class RecipeSet : MonoBehaviour
{
    public List<Recipe> Recipes = new List<Recipe>();

    public void CheckRecipes(Inventory inventory)
    {
        foreach (var recipe in Recipes)
        {
            recipe.CheckIsCompleted(inventory);
        }
    }
}