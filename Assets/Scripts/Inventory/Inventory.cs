using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> items;

    public ReadOnlyCollection<InventoryItem> Items
    {
        get { return items.AsReadOnly(); }
    }
        
    [SerializeField] private Vector2Int size;
    public Vector2Int Size
    {
        get { return size; }
        set 
        { 
            size = value;
            onSizeChange.Invoke();
        }
    }

    public InventoryItemEvent onAddItem;
    public InventoryItemEvent onMoveItem;
    public InventoryItemEvent onDropItem;
    public UnityEvent onSizeChange;

    public InventoryItem this[int index]
    {
        get { return items[index]; }
    }

    public void SetSize(Vector2Int size)
    {
        Size = size;
    }

    public void AddItem(InventoryItem item, Vector2Int position)
    {
        //Validate item is not a duplicate
        if (items.Contains(item))
            throw new ItemMismatchException("Item with same id is already contained in inventory!");
        //Validate position (check collisions)
        BoundsInt destination = FindEmptyBounds(position, item.Size);
        InventoryItem collision = BoundsCollidesStorage(destination);
        if (collision != null)
            throw new InventoryCollisionException(collision);

        item.Position = destination.position.ToVector2Int();
        items.Add(item);
        onAddItem.Invoke(item);
    }

    public void AddItem(InventoryItem item)
    {
        BoundsInt bounds = FindEmptyBounds(item);
        AddItem(item, bounds.position.ToVector2Int());
    }

    public void MoveItem(InventoryItem item, Vector2Int position)
    {
        //Validate item exists in inventory, else add it
        if (!items.Contains(item))
            AddItem(item);
        //Validate position is in bounds
        if(position.x < 0 || position.y < 0 || position.x + item.Size.x > Size.x || position.y + item.Size.y > Size.y)
            throw new InventoryBoundsException("Item is not within bounds!");

        BoundsInt destination = item.Bounds;
        destination.position = position.ToVector3Int();

        //Validate item doesn't collide with another item
        InventoryItem collision = BoundsCollidesStorage(destination, new List<InventoryItem>() {item});
        if (collision != null)
            throw new InventoryCollisionException(collision);

        item.Bounds = destination;
        onMoveItem.Invoke(item);
    }

    public void MoveItem(InventoryItem a, InventoryItem b)
    {
        MoveItem(a, b.Position, b, a.Position);
    }

    public void MoveItem(InventoryItem a, Vector2Int destinationA, InventoryItem b, Vector2Int destinationB)
    {
        //Validate items exists in inventory
        if (!items.Contains(a))
            throw new ItemMismatchException("Item a not found in inventory!");
        if (!items.Contains(b))
            throw new ItemMismatchException("Item b not found in inventory!");

        /*        //Check if item a fits
                BoundsInt bounds = FindEmptyBounds(destinationA, a.Size);
                Item collision = BoundsCollidesStorage(bounds, new List<Item> { a, b });
                if (collision != null)
                    throw new InventoryCollisionException(String.Format("Item collision with {0}", collision.itemName), collision);
                a.Position = bounds.position.ToVector2Int();

                //Check if item b fits
                bounds = FindEmptyBounds(destinationB, b.Size);
                collision = BoundsCollidesStorage(bounds, new List<Item> { a, b });
                if (collision != null)
                    throw new InventoryCollisionException(String.Format("Item collision with {0}", collision.itemName), collision);

                //Swap positions
                b.Position = destinationB;*/

        DropItem(a);
        DropItem(b);

        AddItem(a, destinationA);
        AddItem(b, destinationB);

        onMoveItem.Invoke(a);
        onMoveItem.Invoke(b);
    }

    public InventoryItem GetItem(Vector2Int position, List<InventoryItem> itemMask = null)
    {
        return GetItem(new BoundsInt(position.ToVector3Int(), Vector3Int.one), itemMask);
    }

    public InventoryItem GetItem(BoundsInt bounds, List<InventoryItem> itemMask = null)
    {
        return BoundsCollidesStorage(bounds, itemMask);
    }

    public void DropItem(InventoryItem item)
    {
        if (items.Contains(item))
            items.Remove(item);

        onDropItem.Invoke(item);
    }

    BoundsInt FindEmptyBounds(InventoryItem item)
    {
        return FindEmptyBounds(item.Position, item.Size);
    }

    BoundsInt FindEmptyBounds(Vector2Int itemSize)
    {
        return FindEmptyBounds(Vector2Int.zero, itemSize);
    }

    BoundsInt FindEmptyBounds(Vector2Int position, Vector2Int itemSize)
    {
        if (itemSize.x > Size.x || itemSize.y > size.y)
            throw new InventoryBoundsException(String.Format("Item is too large Item.Size = ({0},{1}); Inventory.Size = ({2},{3})", itemSize.x, itemSize.y, Size.x, Size.y));

        List<BoundsInt> matches = new List<BoundsInt>();
        BoundsInt itemBounds = new BoundsInt();
        itemBounds.SetMinMax(Vector3Int.zero, itemSize.ToVector3Int());

        for (int x = 0; x < (size.x - itemSize.x + 1); x++) //Loop from 0 to max position that item would still be in bounds - x axis
        {
            for (int y = 0; y < (size.y - itemSize.y + 1); y++) //Loop from 0 to max position that item would still be in bounds - y axis
            {
                itemBounds.position = new Vector3Int(x, y, 0);
                itemBounds.size = itemSize.ToVector3Int();

                if (BoundsCollidesStorage(itemBounds) != null)
                    continue;

                matches.Add(itemBounds);
            }
        }

        if(matches.Count < 1)
            throw new InventoryCollisionException("No Valid Position Found");

        return matches.OrderBy((bounds) =>
        {
            return (position - bounds.position.ToVector2Int()).magnitude;
        }).First();
    }

    InventoryItem BoundsCollidesStorage(BoundsInt item, List<InventoryItem> itemMask = null)
    {
        foreach(InventoryItem i in items)
        {
            if (itemMask != null && itemMask.Contains(i))
                continue;

            if (item.Intersects2D(i.Bounds))
                return i;
        }
        return null;
    }

    InventoryItem GetItemAtPosition(Vector2Int position)
    {
        BoundsInt b = new BoundsInt(position.ToVector3Int(), Vector3Int.one);
        foreach (InventoryItem i in items)
        {
            if (b.Intersects2D(i.Bounds))
                return i;
        }
        return null;
    }
}

[System.Serializable]
public class InventoryItemEvent : UnityEvent<InventoryItem> { }