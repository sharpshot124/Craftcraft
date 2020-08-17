using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ItemWizard : MonoBehaviour
{
    [Header("Right click this component for context menu functions")]
    public List<Sprite> SpritesToItemSO = new List<Sprite>();
    public string Path = "Assets/Prefabs/Items/";
    [ContextMenu("Generate Items")]
    public void GenerateItemObjects()
    {
        Item item;
        for (var i = 0; i < SpritesToItemSO.Count; i++)
        {
            Sprite s = SpritesToItemSO[i];
            item = ScriptableObject.CreateInstance<Item>();
            item.InventoryItem = new InventoryItem(Vector2Int.zero, Vector2Int.one) {sprite = s};
            item.name = "item" + i + ".asset";
            item.InventoryItem.SetItemSORef(item);

            AssetDatabase.CreateAsset(item, Path + item.name);
        }
    }

    public List<Item> ItemsToRename = new List<Item>();
    [ContextMenu("Rename Item Asset Names")]
    public void RenameItemAssets()
    {
        foreach (Item item in ItemsToRename)
        {
            item.name = item.InventoryItem.sprite.name;
            item.InventoryItem.itemName = item.name.Trim().ToLower().Replace(' ','_');
            item.InventoryItem.displayName = item.name.ToTitleCase();
            item.InventoryItem.description = String.Format("A rather verbose description of {0}; one that fills the imagination", item.InventoryItem.displayName);

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.name + ".asset");
        }
    }

    public List<Item> ItemsToAddRef = new List<Item>();
    [ContextMenu("Set ItemSO refs")]
    public void SetItemSORefs()
    {
        foreach (Item item in ItemsToRename)
        {
            item.InventoryItem.SetItemSORef(item);
        }
    }
}