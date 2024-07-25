using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryControl : MonoBehaviour
{
    public event Action<int> OnShardsAmountChanged;
    public event Action OnItemBought;
    public event Action OnInventoryItemShowChanged;
    public static InventoryControl inst;

    private int shardCount = 0;
    public int ShardCount { get => shardCount; }

    [Header("Abilities")]
    public List<AbilityItem> abilities;

    [Header("Shop Items")]
    public List<InventoryItem> shopItems;


    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddShards(10);
            HideFromInventory(Utils.Items.Shard);
        }
    }

    public void AddShards(int amount)
    {
        shardCount += amount;
        OnShardsAmountChanged?.Invoke(shardCount);
    }

    public AbilityItem GetAbilityData(Utils.Abilities ability)
    {
        return abilities.Find(x => x.ability == ability);
    }

    public bool BuyItem(Utils.Items itemToPurchase)
    {
        var itemData = shopItems.Find(x => x.item == itemToPurchase);

        if (itemData == null)
            return false;

        if (itemData.Price <= ShardCount && !itemData.IsSoldOut())
        {
            AddShards(-itemData.Price);
            itemData.BuyItem();
            return true;
        }
        return false;
    }

    public void AddItem(Utils.Items item)
    {
        var res = shopItems.Find(x => x.item == item);
        if (res != null)
            res.AddItem();
    }

    public void UseItem(Utils.Items item)
    {
        var res = shopItems.Find(x => x.item == item);
        if (res != null)
            res.UseItem();
    }

    public void ShowItemInInventory(Utils.Items item)
    {
        var res = shopItems.Find(x => x.item == item);
        if (res != null)
            res.isShownInInventory = true;
        OnInventoryItemShowChanged?.Invoke();
    }

    public void HideFromInventory(Utils.Items item)
    {
        var res = shopItems.Find(x => x.item == item);
        if (res != null)
            res.isShownInInventory = false;
        OnInventoryItemShowChanged?.Invoke();
    }
}
