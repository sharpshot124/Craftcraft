using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe", order = 2)]
public class Recipe : ScriptableObject
{
    public string Name;
    public List<Item> Items = new List<Item>();
    public List<Item> ResultItems = new List<Item>();
    public UnityEvent OnCraft = new UnityEvent();

    public void CheckIsCompleted(Inventory inventory)
    {
        InventoryItem[] items = inventory.Items.ToArray();
        InventoryItem[] matches = new InventoryItem[Items.Count];
        bool matchFound;
        Vector2Int offset;
        BoundsInt bounds;

        foreach (InventoryItem item in inventory.Items)
        {
            if (item.itemName == Items[0].InventoryItem.itemName && item.Size == Items[0].InventoryItem.Size)
            {
                offset = item.Position - Items[0].InventoryItem.Position;
                bounds = new BoundsInt((Items[0].InventoryItem.Position + offset).ToVector3Int(), Items[0].InventoryItem.Size.ToVector3Int());
                matchFound = true;
                matches[0] = item;
                for (int r = 1; r < Items.Count; r++)
                {
                    bounds.position = Items[r].InventoryItem.Position.ToVector3Int() + offset.ToVector3Int();
                    bounds.size = Items[r].InventoryItem.Size.ToVector3Int();
                    if ((matches[r] = FindItem(items, Items[r].InventoryItem.itemName, bounds)) != null)
                    {
                        continue;
                    }

                    matchFound = false;
                    break;
                }

                if (matchFound)
                {
                    foreach (InventoryItem ingredient in matches)
                    {
                        inventory.DropItem(ingredient);
                    }

                    for (int i = 0; i < ResultItems.Count; i++)
                    {
                        try
                        {
                            if (i < matches.Length)
                            {
                                inventory.AddItem(new InventoryItem(ResultItems[i]), matches[i].Position);
                            }
                            else
                            {
                                throw new InventoryCollisionException();
                            }
                        }
                        catch (InventoryCollisionException)
                        {
                            inventory.AddItem(new InventoryItem(ResultItems[i]));
                        }
                        OnCraft.Invoke();
                    }
                    return;
                }
            }
        }
    }

    InventoryItem FindItem(InventoryItem[] items, string itemName, BoundsInt bounds)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName && item.Bounds == bounds)
            {
                return item;
            }
        }

        return null;
    }
}
