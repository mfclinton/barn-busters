using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyBuildSystem.Features.Runtime.Buildings.Placer;
using EasyBuildSystem.Features.Runtime.Buildings.Manager;
using EasyBuildSystem.Features.Runtime.Buildings.Part;

public class InputHandler : MonoBehaviour
{
    GameManager gameManager;
    ShopManager shopManager;
    ISelectable selected;

    public delegate void SelectionUpdatedEventHandler(ISelectable selected);
    public event SelectionUpdatedEventHandler OnSelectionUpdated;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        shopManager = FindObjectOfType<ShopManager>();
        shopManager.OnShopItemSelected += (ISelectable newSelected) => UpdateSelected(newSelected);
        BuildingManager.Instance.OnPlacingBuildingPartEvent.AddListener((BuildingPart part) => HandleUseSelected());
    }

    void UpdateSelected(ISelectable newSelected)
    {
        selected = newSelected;
        OnSelectionUpdated?.Invoke(selected);
    }

    void HandleUseSelected()
    {
        if (selected != null)
        {
            bool bought = gameManager.ModifyCash(selected.Cost());
            if(!bought || !gameManager.CanBuy(selected.Cost()))
            {
                selected.DeSelect();
                UpdateSelected(null);
            }

            // TODO: Use?
            // Need to wrap the Builder with a generic class
        }
    }
}
