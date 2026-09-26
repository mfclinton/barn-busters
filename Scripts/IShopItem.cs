using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IShopItem : ScriptableObject
{
    [SerializeField]
    string itemName;
    [SerializeField]
    int cost;
    [SerializeField]
    Sprite sprite;

    public string Name()
    {
        return itemName;
    }

    public int Cost()
    {
        return cost;
    }

    public Sprite GetShopSprite()
    {
        return sprite;
    }
}
