using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IMGUInventory : Editor
{
    private static int itemPickerTarget;

    public static void DrawInventory(Inventory inventory, Rect rect)
    {
        Vector2 cellSize = new Vector2(rect.width / inventory.Size.x, rect.height / inventory.Size.y);
        Rect fieldRect = new Rect();

        bool[,] emptyCells = GetEmptyCells(inventory);
        Item newItem;
        for (int x = 0; x < inventory.Size.x; x++)
        {
            for (int y = 0; y < inventory.Size.y; y++)
            {
                if (!emptyCells[x, y])
                    continue;

                Vector2Int c = new Vector2Int(x, y);
                fieldRect = new Rect(cellSize, cellSize);
                fieldRect.position *= c;
                fieldRect.position += rect.position;

                if (GUI.Button(fieldRect, "null"))
                {
                    itemPickerTarget = CantorEncode(c);
                    EditorGUIUtility.ShowObjectPicker<Item>(null, false, "", itemPickerTarget);
                }
                /* newItem = (ItemSO) EditorGUI.ObjectField(fieldRect, null, typeof(ItemSO), false);
                if (newItem != null)
                {
                    inventory.AddItem(newItem, c);
                } */
            }
        }

        for (int i = 0; i < inventory.Items.Count; i++)
        {
            fieldRect.Set(
                rect.x + cellSize.x * inventory[i].Position.x, rect.y + cellSize.y * inventory[i].Position.y, 
                cellSize.x * inventory[i].Size.x, cellSize.y * inventory[i].Size.y);

            if (GUI.Button(fieldRect, inventory[i].sprite.GetTexture()))
            {
                itemPickerTarget = CantorEncode(inventory[i].Position);
                EditorGUIUtility.ShowObjectPicker<Item>(inventory[i].itemSO, false, "", itemPickerTarget);
            }
            //EditorGUI.DrawPreviewTexture(fieldRect, inventory[i].sprite.GetTexture());         
        }

        //Item Picker changed
        if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == itemPickerTarget)
        {

        }

        //Item Picker closed
        if (Event.current.commandName == "ObjectSelectorClosed" &&
            EditorGUIUtility.GetObjectPickerControlID() == itemPickerTarget)
        {
            var pos = CantorDecode(itemPickerTarget);
            Debug.Log(pos);
            InventoryItem target = inventory.GetItem(pos);

            newItem = (Item)EditorGUIUtility.GetObjectPickerObject();

            if (target != null)
            {
                inventory.DropItem(target);
            }

            if (newItem != null && inventory.GetItem(newItem.InventoryItem.Bounds, new List<InventoryItem> { target }) == null)
            {
                inventory.AddItem(newItem, pos);
            }
            itemPickerTarget = int.MinValue;
            EditorUtility.SetDirty(inventory);
        }
    }

    static bool[,] GetEmptyCells(Inventory inventory)
    {
        bool[,] result = new bool[inventory.Size.x, inventory.Size.y];

        for (int x = 0; x < inventory.Size.x; x++)
        {
            for (int y = 0; y < inventory.Size.y; y++)
            {
                result[x, y] = inventory.GetItem(new Vector2Int(x, y)) == null;
            }
        }

        return result;
    }

    static int CantorEncode(Vector2Int coords)
    {
        return ((coords.x + coords.y) * (coords.x + coords.y + 1)) / 2 + coords.y;
    }

    static Vector2Int CantorDecode(int cantor)
    {
        int t = (int)Mathf.Floor((-1 + Mathf.Sqrt(1 + 8 * cantor)) / 2);
        int x = t * (t + 3) / 2 - cantor;
        int y = cantor - t * (t + 1) / 2;

        return new Vector2Int(x, y);
    }
}