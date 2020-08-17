 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public Inventory inventory;

    [SerializeField]
    private Item contextMenuItem;

    public static InventoryItem Generate(InventoryItem item)
    {
        return new InventoryItem(item);
    }

    public void Generate(Item item)
    {
        if (!inventory)
            return;

        inventory.AddItem(new InventoryItem(item));
    }

    [ContextMenu("Add Context Item to Inventory")]
    void ContextGenerate()
    {
        if(!contextMenuItem)
            return;

        Generate(contextMenuItem);
    }
}
