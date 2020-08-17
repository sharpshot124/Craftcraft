using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPreview(typeof(Item))]
public class ItemSOPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        base.OnPreviewGUI(rect, background);

        Item item = (Item) target;
        GUI.DrawTexture(rect, item.InventoryItem.sprite.GetTexture());
    }
}
