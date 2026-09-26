using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    IShopItem[] shopItems;
    [SerializeField]
    ShopIcon buttonPrefab;
    [SerializeField]
    Transform content;

    GameManager gameManager;
    List<ShopIcon> shopIcons;

    public delegate void ShopItemSelectedEventHandler(ISelectable selected);
    public event ShopItemSelectedEventHandler OnShopItemSelected;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        shopIcons = new List<ShopIcon>();

        GenerateAllShopItem(shopItems, true);

        // Subscribe to Events
        gameManager.OnCashChange += () => UpdateShopIcons(gameManager.Cash());
        UpdateShopIcons(gameManager.Cash()); // Intialize Correctly
    }

    ShopIcon AddItemToShop(IShopItem shopItem)
    {
        ShopIcon newIcon = Instantiate(buttonPrefab, content);
        newIcon.UpdateIcon(shopItem, () => HandleClickedShopItem(newIcon));
        return newIcon;
    }

    void GenerateAllShopItem(IShopItem[] items, bool deleteExisting)
    {
        if(deleteExisting)
        {
            while (content.childCount != 0)
                Destroy(content.GetChild(0));

            shopIcons = new List<ShopIcon>();
        }

        foreach (IShopItem shopItem in items)
            shopIcons.Add(AddItemToShop(shopItem));
    }

    void HandleClickedShopItem(ShopIcon newIcon)
    {
        IShopItem item = newIcon.item;
        if (item is IBuyable)
            ((IBuyable)item).Buy();
        else if (item is ISelectable)
        {
            ISelectable selected = (ISelectable)item;
            selected.Select();
            OnShopItemSelected?.Invoke(selected);
        }
            
    }

    void UpdateShopIcons(int cash)
    {
        foreach(ShopIcon icon in shopIcons)
        {
            icon.SetButtonStatus(icon.item.Cost() <= cash);
        }
    }
}
