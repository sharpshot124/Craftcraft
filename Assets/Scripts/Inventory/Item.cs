using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 1)]
public class Item : ScriptableObject
{
    public InventoryItem InventoryItem;

    public static implicit operator InventoryItem(Item obj)
    {
        return obj.InventoryItem;
    }

    public static Item CreateInstance(InventoryItem item)
    {
        Item result = CreateInstance<Item>();
        result.InventoryItem = item;
        result.InventoryItem.SetItemSORef(result);
        return result;
    }
}
