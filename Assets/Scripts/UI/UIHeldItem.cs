using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHeldItem : MonoBehaviour
{
    public static UIHeldItem inst;

    [Header("Previous")]
    public Image prevBackground;
    public Image prevIcon;
    public TextMeshProUGUI prevCount;

    [Header("Current")]
    public Image curentBackground;
    public Image currentIcon;
    public TextMeshProUGUI currentCount;

    [Header("Next")]
    public Image nextBackground;
    public Image nextIcon;
    public TextMeshProUGUI nextCount;


    private InventoryControl inventory;

    private int itemIndex = 0;

    List<InventoryItem> shownItems = new();

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        inventory = InventoryControl.inst;
        inventory.OnInventoryItemShowChanged += OnItemsChanged;

        inventory.shopItems.ForEach(x => x.OnCountChanged += OnItemCountChanged);

        OnItemsChanged();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            PreviousItem();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            NextItem();
    }

    private void OnItemsChanged()
    {
        shownItems = inventory.shopItems.FindAll(x => x.isShownInInventory && !x.isAbility);
        ShowItems();
    }

    public void NextItem()
    {
        itemIndex++;
        ShowItems();
    }

    public void PreviousItem()
    {
        itemIndex--;
        ShowItems();
    }

    private void ShowItems()
    {
        IndexBoundCheck();


        if (shownItems.Count > 2)
        {
            var prevItem = shownItems[GetPrevIndex()];
            prevIcon.sprite = prevItem.icon;
            prevCount.text = prevItem.OwnedCount.ToString();
            prevBackground.gameObject.SetActive(true);
        }
        else
            prevBackground.gameObject.SetActive(false);
        if (shownItems.Count > 0)
        {
            var currentItem = shownItems[itemIndex];

            currentIcon.sprite = currentItem.icon;
            currentCount.text = currentItem.OwnedCount.ToString();
            curentBackground.gameObject.SetActive(true);
        }
        else
            curentBackground.gameObject.SetActive(false);

        if (shownItems.Count > 1)
        {
            var nextItem = shownItems[GetNextIndex()];
            nextIcon.sprite = nextItem.icon;
            nextCount.text = nextItem.OwnedCount.ToString();
            nextBackground.gameObject.SetActive(true);
        }
        else
            nextBackground.gameObject.SetActive(false);

    }

    private void OnItemCountChanged(InventoryItem source)
    {
        if (shownItems[GetPrevIndex()] == source)
        {
            prevCount.text = source.OwnedCount.ToString();
        }
        else if (shownItems[itemIndex] == source)
        {
            currentCount.text = source.OwnedCount.ToString();
        }
        else if (shownItems[GetNextIndex()] == source)
        {
            nextCount.text = source.OwnedCount.ToString();
        }
    }

    private void IndexBoundCheck()
    {
        if (itemIndex  >= shownItems.Count)
            itemIndex = 0;
        if (itemIndex < 0)
            itemIndex = shownItems.Count - 1;
    }

    private int GetPrevIndex() => itemIndex == 0 ? shownItems.Count - 1 : itemIndex - 1;

    private int GetNextIndex() => itemIndex == shownItems.Count - 1 ? 0 : itemIndex + 1;

    public InventoryItem GetCurrentHeldItem() => shownItems[itemIndex];
}
