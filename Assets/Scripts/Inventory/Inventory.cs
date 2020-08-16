using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> items;

    public ReadOnlyCollection<Item> Items
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

    public Item this[int index]
    {
        get { return items[index]; }
    }

    public void SetSize(Vector2Int size)
    {
        Size = size;
    }

    public void AddItem(Item item, Vector2Int position)
    {
        //Validate item is not a duplicate
        if (items.Contains(item))
            throw new ItemMismatchException("Item with same id is already contained in inventory!");
        //Validate position (check collisions)
        BoundsInt destination = new BoundsInt(position.ToVector3Int(), item.Size.ToVector3Int());
        Item collision = BoundsCollidesStorage(destination);
        if (collision != null)
            throw new InventoryCollisionException(collision);

        item.Position = position;
        items.Add(item);
        onAddItem.Invoke(item);
    }

    public void AddItem(Item item)
    {
        BoundsInt bounds = FindEmptyBounds(item);
        AddItem(item, bounds.position.ToVector2Int());
    }

    public void MoveItem(Item item, Vector2Int position)
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
        Item collision = BoundsCollidesStorage(destination, new List<Item>() {item});
        if (collision != null)
            throw new InventoryCollisionException(collision);

        item.Bounds = destination;
        onMoveItem.Invoke(item);
    }

    public void MoveItem(Item a, Item b)
    {
        //Validate items exists in inventory
        if (!items.Contains(a))
            throw new ItemMismatchException("Item a not found in inventory!");
        if (!items.Contains(b))
            throw new ItemMismatchException("Item b not found in inventory!");

        BoundsInt destination = a.Bounds;
        destination.position = b.Position.ToVector3Int();

        Item collision = BoundsCollidesStorage(destination, new List<Item> { a, b });
        if (collision != null)
            throw new InventoryCollisionException(collision);

        destination = b.Bounds;
        destination.position = a.Position.ToVector3Int();
        collision = BoundsCollidesStorage(destination, new List<Item> { a, b });
        if (collision != null)
            throw new InventoryCollisionException(collision);

        a.Position = b.Position;
        b.Bounds = destination;
        onMoveItem.Invoke(a);
        onMoveItem.Invoke(b);
    }

    public void MoveItem(Item a, Vector2Int destinationA, Item b, Vector2Int destinationB)
    {
        //Validate items exists in inventory
        if (!items.Contains(a))
            throw new ItemMismatchException("Item a not found in inventory!");
        if (!items.Contains(b))
            throw new ItemMismatchException("Item b not found in inventory!");

        BoundsInt bounds = a.Bounds;
        bounds.position = destinationA.ToVector3Int();
        Item collision = BoundsCollidesStorage(bounds, new List<Item> { a, b });
        if (collision != null)
            throw new InventoryCollisionException(collision);

        bounds = b.Bounds;
        bounds.position = destinationB.ToVector3Int();
        collision = BoundsCollidesStorage(bounds, new List<Item> { a, b });
        if (collision != null)
            throw new InventoryCollisionException(collision);

        a.Position = destinationA;
        b.Position = destinationB;
        onMoveItem.Invoke(a);
        onMoveItem.Invoke(b);
    }

    public Item GetItem(Vector2Int position, List<Item> itemMask = null)
    {
        return GetItem(new BoundsInt(position.ToVector3Int(), Vector3Int.one), itemMask);
    }

    public Item GetItem(BoundsInt bounds, List<Item> itemMask = null)
    {
        return BoundsCollidesStorage(bounds, itemMask);
    }

    public void DropItem(Item item)
    {
        if (items.Contains(item))
            items.Remove(item);

        onDropItem.Invoke(item);
    }

    BoundsInt FindEmptyBounds(Item item)
    {
        return FindEmptyBounds(item.Size);
    }

    BoundsInt FindEmptyBounds(Vector2Int itemSize)
    {
        if(itemSize.x > Size.x || itemSize.y > size.y)
            throw new InventoryBoundsException(String.Format("Item is too large Item.Size = ({0},{1}); Inventory.Size = ({2},{3})", itemSize.x, itemSize.y, Size.x, Size.y));

        BoundsInt itemBounds = new BoundsInt();
        itemBounds.SetMinMax(Vector3Int.zero, itemSize.ToVector3Int());

        for(int x = 0; x < (size.x - itemSize.x + 1); x++) //Loop from 0 to max position that item would still be in bounds - x axis
        {
            for(int y = 0; y < (size.y - itemSize.y + 1); y++) //Loop from 0 to max position that item would still be in bounds - y axis
            {
                itemBounds.position = new Vector3Int(x, y, 0);
                itemBounds.size = itemSize.ToVector3Int();

                if (BoundsCollidesStorage(itemBounds) != null)
                    continue;

                return itemBounds;
            }
        }
        throw new InventoryCollisionException("No Valid Position Found");
    }

    Item BoundsCollidesStorage(BoundsInt item, List<Item> itemMask = null)
    {
        foreach(Item i in items)
        {
            if (itemMask != null && itemMask.Contains(i))
                continue;

            if (item.Intersects2D(i.Bounds))
                return i;
        }
        return null;
    }

    Item GetItemAtPosition(Vector2Int position)
    {
        BoundsInt b = new BoundsInt(position.ToVector3Int(), Vector3Int.one);
        foreach (Item i in items)
        {
            if (b.Intersects2D(i.Bounds))
                return i;
        }
        return null;
    }
}

[System.Serializable]
public class InventoryItemEvent : UnityEvent<Item> { }