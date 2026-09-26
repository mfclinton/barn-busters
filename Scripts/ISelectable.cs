using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISelectable : IShopItem
{
    public abstract void Select();
    public abstract void DeSelect();
}
