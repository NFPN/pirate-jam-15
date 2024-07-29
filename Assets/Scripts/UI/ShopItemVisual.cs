using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemVisual : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI descriptionText;
    public Image shardIcon;
    public TextMeshProUGUI priceText;

    private Button button;
    private ShopControl shop;

    public Utils.Items SoldItem { get; set; }

    #region Scaling Variables
    private Vector2 defaultBackgroundSize;
    private Vector2 defaultIconSize;
    private Vector2 defaultDescriptionSize;
    private Vector2 defaultShardIconSize;
    private Vector2 defaultPriceSize;

    private Vector2 iconPosition;
    private Vector2 descriptionPosition;
    private Vector2 shardIconPosition;
    private Vector2 pricePosition;

    private float defaultDescriptionTextSize;
    private float defaultPriceTextSize;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SetDefaultVariables();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnItemClicked);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupItem(ShopControl shop,InventoryItem item)
    {
        this.shop = shop;
        icon.sprite = item.Icon;
        descriptionText.text = item.Description;
        priceText.text = item.Price.ToString();
        SoldItem = item.item;
    }


    private void SetDefaultVariables()
    {
        defaultBackgroundSize = background.rectTransform.sizeDelta;
        defaultIconSize = icon.rectTransform.sizeDelta;
        defaultDescriptionSize = descriptionText.rectTransform.sizeDelta;
        defaultShardIconSize = shardIcon.rectTransform.sizeDelta;
        defaultPriceSize = priceText.rectTransform.sizeDelta;

        defaultDescriptionTextSize = descriptionText.fontSize;
        defaultPriceTextSize = priceText.fontSize;

        iconPosition = icon.rectTransform.anchoredPosition;
        descriptionPosition = descriptionText.rectTransform.anchoredPosition;
        shardIconPosition = shardIcon.rectTransform.anchoredPosition;
        pricePosition = priceText.rectTransform.anchoredPosition;
    }

    public void UpdateScales(Vector2 parentScale)
    {
        var scaleAverage = (parentScale.x + parentScale.y) / 2.0f;

        // Icon scaling
        icon.rectTransform.sizeDelta = new Vector2(defaultIconSize.x * scaleAverage, defaultIconSize.y * scaleAverage);
        icon.rectTransform.anchoredPosition = new(iconPosition.x * parentScale.x, iconPosition.y * parentScale.y);

        // Description scaling
        descriptionText.fontSize = defaultDescriptionTextSize * scaleAverage;
        descriptionText.rectTransform.sizeDelta = new(defaultDescriptionSize.x * parentScale.x, defaultDescriptionSize.y * parentScale.y);
        descriptionText.rectTransform.anchoredPosition = new(descriptionPosition.x * parentScale.x, descriptionPosition.y * parentScale.y);

        // Price text scaling
        priceText.fontSize = defaultPriceTextSize * scaleAverage;
        priceText.rectTransform.sizeDelta = new(defaultPriceSize.x * parentScale.x, defaultPriceSize.y * parentScale.y);
        priceText.rectTransform.anchoredPosition = new(pricePosition.x * parentScale.x, pricePosition.y * parentScale.y);


        // Shard icon scaling
        shardIcon.rectTransform.sizeDelta = new Vector2(defaultShardIconSize.x * scaleAverage, defaultShardIconSize.y * scaleAverage);
        shardIcon.rectTransform.anchoredPosition = new(shardIconPosition.x * parentScale.x, shardIconPosition.y * parentScale.y);

        // Background Scaling
        background.rectTransform.sizeDelta = new Vector2(defaultBackgroundSize.x * parentScale.x, defaultBackgroundSize.y * parentScale.y);

    }



    private void OnMouseEnter()
    {
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIHover);
    }

    private void OnItemClicked()
    {
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIClickSmall);
        shop.OnItemBought(this);
    }
    

    public void SetItemSoldOut()
    {
        print("sold out");
        gameObject.SetActive(false);
    }
}
