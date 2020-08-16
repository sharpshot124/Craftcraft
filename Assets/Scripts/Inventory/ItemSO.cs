using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public Item Item;

    public static implicit operator Item(ItemSO obj)
    {
        return obj.Item;
    }

    public static ItemSO CreateInstance(Item item)
    {
        ItemSO result = CreateInstance<ItemSO>();
        result.Item = item;
        result.Item.SetItemSORef(result);
        return result;
    }
}
