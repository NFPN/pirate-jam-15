using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class InventoryItem
{
    public bool isPurchasable = true;
    public Utils.Items item;

    public Sprite icon;
    public int maxPurchaceCount = 1;
    public bool isAbility;
    public int price;

    private int ownedCount = 0;
    private int itemsSold = 0;

    public int OwnedCount { get => ownedCount; }

    public bool IsSoldOut()
    {
        return !isPurchasable && itemsSold > maxPurchaceCount;
    }

    public void BuyItem(int count = 1)
    {
        ownedCount++;
        itemsSold++;
    }

}
