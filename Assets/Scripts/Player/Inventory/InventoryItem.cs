using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InventoryItem
{
    public bool isPurchasable = true;
    public Utils.Items item;

    public Sprite icon;
    public int price;
    public float priceScaling = 0.2f;
    public string description;
    public int maxPurchaceCount = 1;
    public bool isAbility;

    private int ownedCount = 0;
    private int itemsSold = 0;
    private float defaultPriceScale = 1;

    public int OwnedCount { get => ownedCount; }
    public int SoldCount { get => itemsSold; } 

    public Sprite Icon { get => icon;}
    public int Price { get => CalculatePrice(); }
    public string Description { get => description; }

    public bool IsSoldOut()
    {
        return !isPurchasable || itemsSold >= maxPurchaceCount;
    }

    public void BuyItem()
    {
        ownedCount++;
        itemsSold++;

    }

    public void UseItem()
    {
        ownedCount--;
    }

    private int CalculatePrice()
    {
        return (int)(price * (defaultPriceScale + priceScaling * ownedCount));
    }

}
