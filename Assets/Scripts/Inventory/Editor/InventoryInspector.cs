using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Inventory))]
public class InventoryInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IMGUInventory.DrawInventory((Inventory)target, EditorGUILayout.GetControlRect(false, 300));
    }
}