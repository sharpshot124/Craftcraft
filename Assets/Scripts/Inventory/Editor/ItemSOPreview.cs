using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPreview(typeof(ItemSO))]
public class ItemSOPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        base.OnPreviewGUI(rect, background);

        ItemSO item = (ItemSO) target;
        GUI.DrawTexture(rect, item.Item.sprite.GetTexture());
    }
}
