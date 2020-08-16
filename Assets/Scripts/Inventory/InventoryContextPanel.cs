using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContextPanel : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text content;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Show(Item item)
    {
        gameObject.SetActive(true);
        title.text = item.displayName;
        content.text = item.itemName;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}