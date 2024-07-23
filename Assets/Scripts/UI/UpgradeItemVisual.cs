using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemVisual : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI descriptionText;
    public Image upgradeIcon;
    public TextMeshProUGUI levelText;

    private Button button;
    private UpgradeControl shop;

    public Utils.Abilities UpgradeAbility { get; set; }

    #region Scaling Variables
    private Vector2 defaultBackgroundSize;
    private Vector2 defaultIconSize;
    private Vector2 defaultTitleSize;
    private Vector2 defaultDescriptionSize;
    private Vector2 defaultShardIconSize;
    private Vector2 defaultPriceSize;

    private Vector2 iconPosition;
    private Vector2 titlePosition;
    private Vector2 descriptionPosition;
    private Vector2 shardIconPosition;
    private Vector2 pricePosition;

    private float defaultTitleTextSize;
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

    public void SetupItem(UpgradeControl shop, AbilityItem item)
    {
        this.shop = shop;
        icon.sprite = item.icon;
        title.text = item.name;
        descriptionText.text = item.description;
        levelText.text = $"Lvl {item.Level}";
        UpgradeAbility = item.ability;
    }


    private void SetDefaultVariables()
    {
        defaultBackgroundSize = background.rectTransform.sizeDelta;
        defaultIconSize = icon.rectTransform.sizeDelta;
        defaultDescriptionSize = descriptionText.rectTransform.sizeDelta;
        defaultTitleSize = title.rectTransform.sizeDelta;
        //defaultShardIconSize = upgradeIcon.rectTransform.sizeDelta;
        defaultPriceSize = levelText.rectTransform.sizeDelta;

        defaultTitleTextSize = title.fontSize;
        defaultDescriptionTextSize = descriptionText.fontSize;
        defaultPriceTextSize = levelText.fontSize;

        iconPosition = icon.rectTransform.anchoredPosition;
        titlePosition = title.rectTransform.anchoredPosition;
        descriptionPosition = descriptionText.rectTransform.anchoredPosition;
        //shardIconPosition = upgradeIcon.rectTransform.anchoredPosition;
        pricePosition = levelText.rectTransform.anchoredPosition;
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
        
        // Title scaling
        title.fontSize = defaultTitleTextSize * scaleAverage;
        title.rectTransform.sizeDelta = new(defaultTitleSize.x * parentScale.x, defaultTitleSize.y * parentScale.y);
        title.rectTransform.anchoredPosition = new(titlePosition.x * parentScale.x, titlePosition.y * parentScale.y);

        // Price text scaling
        levelText.fontSize = defaultPriceTextSize * scaleAverage;
        levelText.rectTransform.sizeDelta = new(defaultPriceSize.x * parentScale.x, defaultPriceSize.y * parentScale.y);
        levelText.rectTransform.anchoredPosition = new(pricePosition.x * parentScale.x, pricePosition.y * parentScale.y);


        // Shard icon scaling
        //upgradeIcon.rectTransform.sizeDelta = new Vector2(defaultShardIconSize.x * scaleAverage, defaultShardIconSize.y * scaleAverage);
        //upgradeIcon.rectTransform.anchoredPosition = new(shardIconPosition.x * parentScale.x, shardIconPosition.y * parentScale.y);

        // Background Scaling
        background.rectTransform.sizeDelta = new Vector2(defaultBackgroundSize.x * parentScale.x, defaultBackgroundSize.y * parentScale.y);

    }

    private void OnItemClicked()
    {
        shop.OnAbilityUpgraded(this);
    }
}
