using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string displayName, itemName, description;
    public Sprite sprite;
    public Color Color = Color.white;
    public Item itemSO = null;

    Guid id;
    public Guid ID
    { get { return id; } }

    [SerializeField]
    BoundsInt2D bounds;
    public BoundsInt Bounds
    {
        get { return bounds; }
        set { bounds = value; }
    }
    public Vector2Int Position
    {
        get { return Bounds.position.ToVector2Int(); }
        set { bounds.Position = value; }
    }
    public Vector2Int Size
    {
        get { return Bounds.size.ToVector2Int(); }
        set { bounds.Size = value; }
    }

    public void SetItemSORef(Item itemSO)
    {
        this.itemSO = itemSO;
    }

    public InventoryItem()
    {
        this.id = new Guid();
    }

    public InventoryItem(Vector2Int size)
    {
        this.id = new Guid();
        this.Size = size;
    }

    public InventoryItem(Vector2Int position, Vector2Int size)
    {
        this.id = new Guid();
        this.Position = position;
        this.Size = size;
    }

    public InventoryItem(BoundsInt bounds)
    {
        this.id = new Guid();
        this.Bounds = bounds;
    }

    public InventoryItem(InventoryItem item)
    {
        this.id = new Guid();
        this.Bounds = item.Bounds;
        this.itemName = item.itemName;
        this.displayName = item.displayName;
        this.description = item.description;
        this.sprite = item.sprite;
        this.Color = item.Color;
    }
}

[Serializable]
struct BoundsInt2D
{
    [SerializeField] private Vector2Int position;
    public Vector2Int Position
    {
        get { return position; }
        set { position = value; }
    }

    [SerializeField] private Vector2Int size;
    public Vector2Int Size
    {
        get { return size; }
        set { size = value; }
    }

    public BoundsInt2D(BoundsInt bounds)
    {
        this.position = bounds.position.ToVector2Int();
        this.size = bounds.size.ToVector2Int();
    }

    public static implicit operator BoundsInt(BoundsInt2D bounds)
    {
        return new BoundsInt(bounds.position.ToVector3Int(), bounds.size.ToVector3Int());
    }

    public static implicit operator BoundsInt2D(BoundsInt bounds)
    {
        return new BoundsInt2D(bounds);
    }
}