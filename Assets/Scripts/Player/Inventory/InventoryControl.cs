using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryControl : MonoBehaviour
{
    public event Action<int> OnShardsAmountChanged;
    public event Action OnItemBought;
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
            inst = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
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
}
