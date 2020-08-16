using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe", order = 2)]
public class Recipe : ScriptableObject
{
    public string Name;
    public List<ItemSO> Items = new List<ItemSO>();
    public List<ItemSO> ResultItems = new List<ItemSO>();
    public UnityEvent OnCraft = new UnityEvent();

    public void CheckIsCompleted(Inventory inventory)
    {
        Item[] items = inventory.Items.ToArray();
        Item[] matches = new Item[Items.Count];
        bool matchFound;
        Vector2Int offset;
        BoundsInt bounds;

        foreach (Item item in inventory.Items)
        {
            if (item.itemName == Items[0].Item.itemName && item.Size == Items[0].Item.Size)
            {
                offset = item.Position - Items[0].Item.Position;
                bounds = new BoundsInt((Items[0].Item.Position + offset).ToVector3Int(), Items[0].Item.Size.ToVector3Int());
                matchFound = true;
                matches[0] = item;
                for (int r = 1; r < Items.Count; r++)
                {
                    bounds.position = Items[r].Item.Position.ToVector3Int() + offset.ToVector3Int();
                    bounds.size = Items[r].Item.Size.ToVector3Int();
                    if ((matches[r] = FindItem(items, Items[r].Item.itemName, bounds)) != null)
                    {
                        continue;
                    }

                    matchFound = false;
                    break;
                }

                if (matchFound)
                {
                    foreach (Item ingredient in matches)
                    {
                        inventory.DropItem(ingredient);
                    }

                    for (int i = 0; i < ResultItems.Count; i++)
                    {
                        try
                        {
                            if (i < matches.Length)
                            {
                                inventory.AddItem(new Item(ResultItems[i]), matches[i].Position);
                            }
                            else
                            {
                                throw new InventoryCollisionException();
                            }
                        }
                        catch (InventoryCollisionException)
                        {
                            inventory.AddItem(new Item(ResultItems[i]));
                        }
                        OnCraft.Invoke();
                    }
                    return;
                }
            }
        }
    }

    Item FindItem(Item[] items, string itemName, BoundsInt bounds)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName && item.Bounds == bounds)
            {
                return item;
            }
        }

        return null;
    }
}
