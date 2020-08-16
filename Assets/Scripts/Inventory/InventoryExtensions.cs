using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class InventoryExtensions
{
    public static bool Intersects2D(this BoundsInt a, BoundsInt b)
    {
        return (a.x < b.x + b.size.x &&
            a.x + a.size.x > b.x &&
            a.y < b.y + b.size.y &&
            a.y + a.size.y > b.y);
    }

    public static Vector3Int ToVector3Int(this Vector2Int vector)
    {
        return new Vector3Int(vector.x, vector.y, 0);
    }

    public static Vector2Int ToVector2Int(this Vector3Int vector)
    {
        return new Vector2Int(vector.x, vector.y);
    }

    public static void AddEvent(this EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry {eventID = type};
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    public static TKey GetKeyFromValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
    {
        List<TKey> keys = new List<TKey>(dict.Keys);
        int index = new List<TValue>(dict.Values).IndexOf(value);

        return keys[index];
    }

    public static Texture2D GetTexture(this Sprite sprite)
    {
        if (!sprite)
            return Texture2D.blackTexture;

        var r = new RectInt((int)sprite.textureRect.x, (int)sprite.textureRect.y,
            (int)sprite.textureRect.width, (int)sprite.textureRect.height);

        Texture2D result = new Texture2D(r.width, r.height);
        result.SetPixels(sprite.texture.GetPixels(r.x, r.y, r.width, r.height));
        result.Apply();
        return result;
    }
}
