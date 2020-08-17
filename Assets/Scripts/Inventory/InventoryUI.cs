using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]
public class InventoryUI : MonoBehaviour {

    private RectTransform transform2D;
    private Dictionary<RectTransform, InventoryItem> items = new Dictionary<RectTransform, InventoryItem>();
    [SerializeField] private GameObject templateCell, templateItem;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GridLayoutGroup cellContainer;
    [SerializeField] private RectTransform itemContainer;
    [SerializeField] private InventoryContextPanel context;

    public Inventory Inventory
    {
        get { return inventory; }
        set
        {
            inventory = value;
            DrawUI();
        }
    }
    /// <summary>
    /// Returns component-wise scaling of Transform2D.sizeDelta divided by Inventory.Size;
    /// </summary>
    private Vector2 CellSize
    {
        get { return new Vector2(transform2D.rect.width / inventory.Size.x, transform2D.rect.height / inventory.Size.y); }
    }
    /// <summary>
    /// CellSize scaled by (1,-1)
    /// </summary>
    private Vector2 CellPosition
    {
        get { return Vector2.Scale(CellSize, new Vector2(1, -1)); }
    }

	/// <summary>
    /// Initialization
    /// </summary>
	void Start ()
    {
        transform2D = cellContainer.GetComponent<RectTransform>();

        if (Inventory)
            DrawUI();
	}
    /// <summary>
    /// Draw (or redraw) cells and items based on CellSize
    /// </summary>
    public void DrawUI()
    {
        DrawCells();

        var existingItems = new List<InventoryItem>(items.Values);
        foreach(var item in inventory.Items)
        {
            if(!existingItems.Contains(item))
            {
                AddItem(item);
            }
        }

        DrawItems();
    }
    /// <summary>
    /// Destroy extra cells, create needed cells, set up GridLayoutGroup according to CellSize
    /// </summary>
    void DrawCells()
    {
        cellContainer.cellSize = CellSize;
        cellContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cellContainer.constraintCount = inventory.Size.x;

        RectTransform cell = transform2D;

        //Destroy any extra cells
        for (int i = inventory.Size.x * inventory.Size.y; i < cell.childCount; i++)
        {
            Destroy(cell.GetChild(i).gameObject);
        }

        //Loop through cells; reuse old ones or create new ones; set properties
        int index;
        for (int x = 0; x < inventory.Size.x; x++)
        {
            for (int y = 0; y < inventory.Size.y; y++)
            {
                //check if cell already exists, else create new one
                index = x * inventory.Size.y + y;
                if (index < cellContainer.transform.childCount)
                    cell = (RectTransform)cellContainer.transform.GetChild(index);
                else
                    cell = (RectTransform)Instantiate(templateCell).transform;

                //set properties
                cell.name = string.Format("Cell ({0}, {1})", x, y);
                cell.SetParent(cellContainer.transform);
                cell.sizeDelta = CellSize;
                cell.localScale = Vector3.one;
            }
        }
    }

    /// <summary>
    /// Move and resize item sprites according to CellSize
    /// </summary>
    void DrawItems()
    {
        //Resize/reposition item sprites in case of change in CellSize
        foreach (RectTransform sprite in new List<RectTransform>(items.Keys))
        {
            sprite.SetParent(itemContainer);
            sprite.localScale = Vector3.one;
            sprite.sizeDelta = Vector2.Scale(CellSize, items[sprite].Size);
            sprite.localPosition = GetCellUIPosition(items[sprite].Position, false);
        }
    }

    /// <summary>
    /// Returns the transform position associated with a given inventory position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="worldSpace"></param>
    /// <returns>Vector3 value representing a transform position</returns>
    public Vector3 GetCellUIPosition(Vector2Int position, bool worldSpace = true)
    {
        Vector3 pos = Vector2.Scale(CellPosition, position);

        if (worldSpace)
        {
            return pos + transform2D.position;
        }
        else
        {
            return pos;
        }
    }
    /// <summary>
    /// Returns floored values of component-wise scaling of position and CellSize. In effect, a mapping of transform positions to inventory positions.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="worldSpace"></param>
    /// <returns>Vector2Int value representing a position in an inventory based on CellSize</returns>
    public Vector2Int GetCellInventoryPosition(Vector2 position, bool worldSpace = true)
    {
        if (worldSpace)
        {
            position = transform2D.InverseTransformPoint(position);
        }

        position.Scale(new Vector2(1 / CellSize.x, -1 / CellSize.y));

        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
    }

    /// <summary>
    /// Adds an item sprite based item input
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(InventoryItem item)
    {
        RectTransform spriteObj = Instantiate(templateItem).GetComponent<RectTransform>();
        Image sprite = spriteObj.GetComponent<Image>();
        
        sprite.sprite = item.sprite;
        sprite.color = item.Color;
        spriteObj.name = item.itemName;

        //Add item events
        EventTrigger trigger = spriteObj.GetComponent<EventTrigger>();
        trigger.AddEvent(EventTriggerType.Drag, (data) => OnItemDragHandler((PointerEventData)data));
        trigger.AddEvent(EventTriggerType.EndDrag, (data) => OnItemEndDragHandler((PointerEventData)data));
        trigger.AddEvent(EventTriggerType.PointerEnter, (data) => OnItemMouseEnter((PointerEventData)data));
        trigger.AddEvent(EventTriggerType.PointerExit, (data) => OnItemMouseExit((PointerEventData)data));

        items.Add(spriteObj, item);
        DrawItems();
    }

    /// <summary>
    /// Repositions the sprite for the given item
    /// </summary>
    /// <param name="item"></param>
    public void MoveItem(InventoryItem item)
    {
        DrawItems();
    }

    /// <summary>
    /// Destroys the sprite for a given item
    /// </summary>
    /// <param name="item"></param>
    public void DropItem(InventoryItem item)
    {
        RectTransform sprite = items.GetKeyFromValue(item);
        items.Remove(sprite);
        Destroy(sprite.gameObject);
    }

    void OnItemDragHandler(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        RectTransform item = data.pointerDrag.GetComponent<RectTransform>();
        context.Hide();

        //Determine target inventory
        InventoryUI target = null;
        foreach (GameObject h in data.hovered)
        {
            target = h.GetComponent<InventoryUI>();
            //Inventory found
            if (target)
                break;
        }

        item.GetComponent<Image>().enabled = target;
        if (!target)
            return;

        //set properties
        item.SetParent(target.transform);
        item.SetAsLastSibling();
        item.sizeDelta = Vector2.Scale(target.CellSize, items[item].Size);
        item.localPosition = target.GetCellUIPosition(target.GetCellInventoryPosition(data.position), false);
    }

    void OnItemEndDragHandler(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        RectTransform sprite = data.pointerDrag.GetComponent<RectTransform>();

        InventoryItem item = items[sprite];
        sprite.GetComponent<Image>().enabled = true;

        //Determine Target
        InventoryUI target = null;
        foreach (GameObject h in data.hovered)
        {
            target = h.GetComponent<InventoryUI>();
            //Inventory found
            if (target)
                break;
        }

        if (!target)
            //target = this
            return;

        var destination = target.GetCellInventoryPosition(data.position);
        if (target == this) //Hovering over same inventory, move item position
        {
            try
            {
                inventory.MoveItem(item, destination);
            }
            catch (InventoryCollisionException e)
            {
                //Another item is in the way, try to swap item positions
                try
                {
                    inventory.MoveItem(item, destination, e.Item, item.Position);
                }
                catch (InventoryCollisionException e2)
                {
                    sprite.GetComponent<Image>().enabled = true;
                    DrawItems();
                    throw e2;
                }
            }
        }
        else //Hovering over a different inventory, move item to new one
        {
            inventory.DropItem(item);
            try
            {                
                target.inventory.AddItem(item, destination);
            }
            catch (InventoryCollisionException e)
            {
                target.inventory.DropItem(e.Item);

                try
                {
                    inventory.AddItem(e.Item);
                }
                catch(InventoryCollisionException e2)
                {
                    //Swap failed return both items
                    inventory.AddItem(item, item.Position);
                    target.inventory.AddItem(e.Item, e.Item.Position);

                    sprite.GetComponent<Image>().enabled = true;
                    target.DrawItems();
                    throw e2;
                }

                target.inventory.AddItem(item, destination);
            }
        }

        target.DrawItems();
    }

    void OnItemMouseEnter(PointerEventData data)
    {
        StartCoroutine(FollowMouse(context.GetComponent<RectTransform>()));
        context.Show(items[data.pointerEnter.GetComponent<RectTransform>()]);
    }

    void OnItemMouseExit(PointerEventData data)
    {
        StopCoroutine(FollowMouse(context.GetComponent<RectTransform>()));
        context.Hide();
    }

    IEnumerator FollowMouse(RectTransform target)
    {
        while (true)
        {
            target.position = Input.mousePosition;
            yield return new WaitForEndOfFrame();
        }
    }
}