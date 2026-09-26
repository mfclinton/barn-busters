using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateClean;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(CleanButton))]
public class ShopIcon : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    TextMeshProUGUI costText;

    public IShopItem item { get; private set; }
    CleanButton button;

    private void Awake()
    {
        button = GetComponent<CleanButton>();
    }

    public void UpdateIcon(IShopItem item, UnityAction onSelectedEvent)
    {
        costText.text = $"{item.Cost()}";
        image.sprite = item.GetShopSprite();
        this.item = item;
        button.onClick.AddListener(onSelectedEvent);
    }

    public void SetButtonStatus(bool enabled)
    {
        button.interactable = enabled;
    }
}
