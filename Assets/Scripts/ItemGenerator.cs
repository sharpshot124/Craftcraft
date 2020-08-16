 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public Inventory inventory;

    [SerializeField]
    private ItemSO contextMenuItem;

    public static Item Generate(Item item)
    {
        return new Item(item);
    }

    public void Generate(ItemSO item)
    {
        if (!inventory)
            return;

        inventory.AddItem(new Item(item));
    }

    [ContextMenu("Add Context Item to Inventory")]
    void ContextGenerate()
    {
        if(!contextMenuItem)
            return;

        Generate(contextMenuItem);
    }
}
