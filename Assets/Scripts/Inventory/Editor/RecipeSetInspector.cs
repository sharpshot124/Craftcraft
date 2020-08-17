using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(RecipeSet))]
public class RecipeSetInspector : Editor
{
    private int index = 0;
    private bool recipeItemsExpanded = true;
    private bool resultItemsExpanded = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RecipeSet set = (RecipeSet)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<"))
            index--;
        if (GUILayout.Button("Add Recipe"))
        {
            Recipe r = CreateInstance<Recipe>();
            AssetDatabase.CreateFolder("Assets/Prefabs/Recipes", "New Recipe");
            AssetDatabase.CreateAsset(r, "Assets/Prefabs/Recipes/New Recipe/" + "NewRecipe.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            ProjectWindowUtil.ShowCreatedAsset(r);

            set.Recipes.Add(r);
            index = set.Recipes.Count - 1;
        }
        if (GUILayout.Button(">>"))
            index++;
        GUILayout.EndHorizontal();

        if (set.Recipes.Count == 0)
            return;
        else
            index = Mathf.Clamp(index, 0, set.Recipes.Count - 1);
        Recipe recipe = set.Recipes[index];
        if(recipe == null)
            return;

        EditorGUILayout.Separator();
        recipe.Name = EditorGUILayout.TextField(string.Format("Recipe ({0}/{1}) Name: ", index + 1, set.Recipes.Count), recipe.Name);

        if (recipeItemsExpanded = EditorGUILayout.Foldout(recipeItemsExpanded, "Ingredients"))
        {
            for(int i = 0; i < recipe.Items.Count; i++)
            {
                recipe.Items[i] = (Item)EditorGUILayout.ObjectField(recipe.Items[i], typeof(Item), false);
            }

            if (GUILayout.Button("Add Ingredient"))
            {
                Item obj = CreateInstance<Item>();
                if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Recipes/" + recipe.Name))
                    AssetDatabase.CreateFolder("Assets/Prefabs/Recipes", recipe.Name);
                AssetDatabase.CreateAsset(obj, string.Format("Assets/Prefabs/Recipes/{0}/{1}", recipe.Name, "NewItem.asset"));
                AssetDatabase.SaveAssets();

                recipe.Items.Add(obj);
            }
        }
        if (resultItemsExpanded = EditorGUILayout.Foldout(resultItemsExpanded, "Result Items"))
        {
            for (int i = 0; i < recipe.ResultItems.Count; i++)
            {
                recipe.ResultItems[i] = (Item)EditorGUILayout.ObjectField(recipe.ResultItems[i], typeof(Item), false);
            }

            if (GUILayout.Button("Add Result Item"))
            {

                recipe.ResultItems.Add(null);
            }
        }
        EditorGUILayout.Separator();

        if (GUILayout.Button("Delete Recipe"))
        {
            set.Recipes.Remove(recipe);
        }
    }
}