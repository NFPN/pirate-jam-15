using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopControl : MonoBehaviour
{
    [Header("Shop Background")]
    public Image backgroundImage;
    public Vector2 backgroundScale = new Vector2(0.75f, 0.75f);
    public float backgroundTransparency = 0.7f;
    public RectMask2D scrollbarMask;

    private Vector2 previousScreenSize;

    private Vector2 defaultSize;
    private Vector2 defaultMaskSoftness;
    private Rect defaultBackgroundRect;
    private Vector2 anchoredBackgroundPosition;
    private float defaultShopItemSpacing;

    private Vector2 sizeCoeficient = new Vector2(1, 1);

    private Canvas shopCanvas;

    InventoryControl inventoryControl;

    [Header("Shop Item")]
    public RectTransform shopItemHolder;
    public GameObject itemPrefab;
    public List<ShopItemVisual> shopItems;

    private VerticalLayoutGroup layout;

    // Start is called before the first frame update
    void Start()
    {
        shopCanvas = GetComponent<Canvas>();
        layout = shopItemHolder.GetComponent<VerticalLayoutGroup>();

        defaultSize = new Vector2(1920, 1080);
        defaultBackgroundRect = backgroundImage.rectTransform.rect;
        anchoredBackgroundPosition = backgroundImage.rectTransform.anchoredPosition;

        defaultMaskSoftness = scrollbarMask.softness;
        defaultShopItemSpacing = layout.spacing;


        inventoryControl = InventoryControl.inst;

        SetupShopItems(inventoryControl);
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
                item.SetItemSoldOut();
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
        shopItems.ForEach(item => item.UpdateScales(sizeCoeficient));
        UpdateItemHolderHeight();
    }


    private void SetupShopItems(InventoryControl inventory)
    {
        foreach (var item in inventory.shopItems)
        {
            if(item.isPurchasable && !item.IsSoldOut())
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
            shopItemHolder.sizeDelta = new(shopItemHolder.sizeDelta.x, (layout.spacing +layout.padding.top + shopItems[0].background.rectTransform.sizeDelta.y) * shopItems.Count);
    }
}
