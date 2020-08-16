using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ItemWizard : MonoBehaviour
{
    public List<Sprite> SpritesToItemSO = new List<Sprite>();
    public string Path = "Assets/Prefabs/Items/";
    [ContextMenu("Generate Items")]
    public void GenerateItemObjects()
    {
        ItemSO item;
        for (var i = 0; i < SpritesToItemSO.Count; i++)
        {
            Sprite s = SpritesToItemSO[i];
            item = ScriptableObject.CreateInstance<ItemSO>();
            item.Item = new Item(Vector2Int.zero, Vector2Int.one) {sprite = s};
            item.name = "item" + i + ".asset";

            AssetDatabase.CreateAsset(item, Path + item.name);
        }
    }

    public List<ItemSO> ItemsToRename = new List<ItemSO>();
    [ContextMenu("Rename Item Asset Names")]
    public void RenameItemAssets()
    {
        foreach (ItemSO item in ItemsToRename)
        {
            item.name = item.Item.itemName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(item), item.name + ".asset");
        }
    }

    public List<ItemSO> ItemsToAddRef = new List<ItemSO>();
    [ContextMenu("Set ItemSO refs")]
    public void SetItemSORefs()
    {
        foreach (ItemSO item in ItemsToRename)
        {
            item.Item.SetItemSORef(item);
        }
    }
}