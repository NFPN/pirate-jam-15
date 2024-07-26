using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopControl : MonoBehaviour
{
    [Header("Shop Background")]
    public Image backgroundImage;
    public Vector2 backgroundScale = new Vector2(0.75f, 0.75f);
    public float backgroundTransparency = 0.7f;
    public RectMask2D scrollbarMask;
    private RectTransform scrollbar;


    private Vector2 previousScreenSize;

    private Vector2 defaultSize;
    private Vector2 defaultMaskSoftness;
    private Rect defaultBackgroundRect;
    private Vector2 anchoredBackgroundPosition;
    private float defaultShopItemSpacing;

    private Vector2 defaultScrollPosition;
    private Vector2 defaultScrollSize;
    private RectOffset defaultLayoutOffset;

    private Vector2 sizeCoeficient = new Vector2(1, 1);

    private Canvas shopCanvas;

    InventoryControl inventoryControl;

    [Header("Shop Item")]
    public RectTransform shopItemHolder;
    public GameObject itemPrefab;
    public List<ShopItemVisual> shopItems;

    private VerticalLayoutGroup layout;

    private bool isShopOpen = false;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        shopCanvas = GetComponent<Canvas>();
        layout = shopItemHolder.GetComponent<VerticalLayoutGroup>();
        scrollbar = scrollbarMask.GetComponent<RectTransform>();

        defaultSize = new Vector2(1920, 1080);
        defaultBackgroundRect = backgroundImage.rectTransform.rect;
        anchoredBackgroundPosition = backgroundImage.rectTransform.anchoredPosition;

        defaultMaskSoftness = scrollbarMask.softness;
        defaultShopItemSpacing = layout.spacing;

        defaultScrollPosition = scrollbar.anchoredPosition;
        defaultScrollSize = scrollbar.sizeDelta;

        defaultLayoutOffset = layout.padding;


        inventoryControl = InventoryControl.inst;

        if (DataControl.inst != null)
            DataControl.inst.OnLoaded += OnSceneLoaded;

        CloseShop();

        InputControl.inst.Subscribe("Shop", ChangeShopState);
        InputControl.inst.Subscribe("ExitWindow", OnCloseUIKey);
    }

    private void OnSceneLoaded()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void OnEnable()
    {
        if (InputControl.inst != null)
        {
            InputControl.inst.Subscribe("Shop", ChangeShopState);
            InputControl.inst.Subscribe("ExitWindow", OnCloseUIKey);

        }
    }

    private void OnDisable()
    {
        InputControl.inst.Unsubscribe("Shop", ChangeShopState);
        InputControl.inst.Unsubscribe("ExitWindow", OnCloseUIKey);

    }
    // Update is called once per frame
    void Update()
    {
        if (shopCanvas.pixelRect.width != previousScreenSize.x || shopCanvas.pixelRect.height != previousScreenSize.y)
        {
            ScaleShopUI();
        }
    }

    public void OnItemBought(ShopItemVisual item)
    {
        // returns true if bought successfully 
        if (inventoryControl.BuyItem(item.SoldItem))
        {
            var targetItem = inventoryControl.shopItems.Find(x => x.item == item.SoldItem);
            if (targetItem.IsSoldOut())
            {
                shopItems.Remove(item);
                Destroy(item.gameObject);
            }
            else
                item.SetupItem(this, targetItem);
        }

    }

    private void ScaleShopUI()
    {
        // update previous screen size
        previousScreenSize = new Vector2(shopCanvas.pixelRect.width, shopCanvas.pixelRect.height);
        sizeCoeficient = new Vector2(previousScreenSize.x / defaultSize.x, previousScreenSize.y / defaultSize.y);

        backgroundImage.rectTransform.sizeDelta = new(defaultBackgroundRect.width * sizeCoeficient.x, defaultBackgroundRect.height * sizeCoeficient.y);
        backgroundImage.rectTransform.anchoredPosition = new Vector3(anchoredBackgroundPosition.x * sizeCoeficient.x, anchoredBackgroundPosition.y * sizeCoeficient.y, 0);

        scrollbarMask.softness = new((int)(defaultMaskSoftness.x * sizeCoeficient.x), (int)(defaultMaskSoftness.y * sizeCoeficient.y));
        layout.spacing = defaultShopItemSpacing * sizeCoeficient.y;

        scrollbar.anchoredPosition = new Vector2(defaultScrollPosition.x * sizeCoeficient.x, defaultScrollPosition.y * sizeCoeficient.y);
        scrollbar.sizeDelta = new Vector2(defaultScrollSize.x * sizeCoeficient.x, defaultScrollSize.y * sizeCoeficient.y);

        layout.padding = new((int)(defaultLayoutOffset.left * sizeCoeficient.x), (int)(defaultLayoutOffset.right * sizeCoeficient.x), (int)(defaultLayoutOffset.top * sizeCoeficient.y), (int)(defaultLayoutOffset.bottom * sizeCoeficient.y));

        shopItems.ForEach(item => item.UpdateScales(sizeCoeficient));
        UpdateItemHolderHeight();
    }


    private void SetupShopItems(InventoryControl inventory)
    {
        foreach (var item in inventory.shopItems)
        {
            if (item.isPurchasable && !item.IsSoldOut())
            {
                var sellable = Instantiate(itemPrefab, shopItemHolder);
                var itemVisual = sellable.GetComponent<ShopItemVisual>();
                itemVisual.SetupItem(this, item);
                shopItems.Add(itemVisual);
            }
        }

        UpdateItemHolderHeight();
    }

    private void UpdateItemHolderHeight()
    {
        if (shopItems.Count > 0)
            shopItemHolder.sizeDelta = new(shopItemHolder.sizeDelta.x, (layout.spacing + layout.padding.top + shopItems[0].background.rectTransform.sizeDelta.y) * shopItems.Count);
    }

    public void OpenShop()
    {
        if (player != null)
            player.DisablePlayerControls(true);

        isShopOpen = true;
        inventoryControl.WindowOpen = isShopOpen;
        ClearShopItems();
        SetupShopItems(inventoryControl);
        backgroundImage.gameObject.SetActive(true);
    }
    public void CloseShop()
    {
        if (player != null)
            player.DisablePlayerControls(false);

        isShopOpen = false;
        inventoryControl.WindowOpen = isShopOpen;
        ClearShopItems();
        backgroundImage.gameObject.SetActive(false);
    }

    private void ClearShopItems()
    {
        foreach (var item in shopItems)
        {
            Destroy(item.gameObject);
        }
        shopItems.Clear();
    }

    public void ChangeShopState(InputAction.CallbackContext callback)
    {
        if (inventoryControl.WindowOpen && !isShopOpen)
            return;
        if (callback.phase == InputActionPhase.Started)
        {
            if (isShopOpen)
                CloseShop();
            else
                OpenShop();
        }
    }

    public void OnCloseUIKey(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        if (isShopOpen)
        {
            CloseShop();
        }
    }
}
