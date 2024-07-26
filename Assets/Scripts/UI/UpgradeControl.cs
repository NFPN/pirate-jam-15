using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeControl : MonoBehaviour
{
    [Header("Upgrade Background")]
    public Image backgroundImage;
    public TextMeshProUGUI upgradeText;
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
    private Vector2 defaultUpgradeTextPosition;
    private float defaultUpgradeTextsize;
    private Vector2 defaultScrollPosition;
    private Vector2 defaultScrollSize;

    private RectOffset defaultLayoutOffset;


    private Vector2 sizeCoeficient = new Vector2(1, 1);

    private Canvas shopCanvas;

    InventoryControl inventoryControl;

    [Header("Upgrade Item")]
    public RectTransform shopItemHolder;
    public GameObject itemPrefab;
    public List<UpgradeItemVisual> upgradeItems;

    private VerticalLayoutGroup layout;

    private bool isUpgradesOpen = false;

    private Player player;


    // Start is called before the first frame update
    void Start()
    {
        shopCanvas = GetComponent<Canvas>();
        layout = shopItemHolder.GetComponent<VerticalLayoutGroup>();
        scrollbar = scrollbarMask.GetComponent<RectTransform>();

        #region default sizes
        defaultSize = new Vector2(1920, 1080);
        defaultBackgroundRect = backgroundImage.rectTransform.rect;
        anchoredBackgroundPosition = backgroundImage.rectTransform.anchoredPosition;

        defaultMaskSoftness = scrollbarMask.softness;
        defaultShopItemSpacing = layout.spacing;

        defaultUpgradeTextsize = upgradeText.fontSize;
        defaultScrollPosition = scrollbar.anchoredPosition;
        defaultScrollSize = scrollbar.sizeDelta;
        defaultUpgradeTextPosition = upgradeText.rectTransform.anchoredPosition;

        defaultLayoutOffset = layout.padding;


        #endregion


        inventoryControl = InventoryControl.inst;

        InputControl.inst.Subscribe("ExitWindow", OnCloseUIKey);

        if (DataControl.inst != null)
            DataControl.inst.OnLoaded += OnSceneLoaded;


        CloseUpgrades();
        SetupShopItems(inventoryControl);
    }

    private void OnSceneLoaded()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void OnEnable()
    {
        if(InputControl.inst != null)
            InputControl.inst.Subscribe("ExitWindow", OnCloseUIKey);

    }
    private void OnDisable()
    {
        InputControl.inst.Unsubscribe("ExitWindow", OnCloseUIKey);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeUpgradeState();
        }

        if (shopCanvas.pixelRect.width != previousScreenSize.x || shopCanvas.pixelRect.height != previousScreenSize.y)
        {
            ScaleUpgradeUI();
        }
    }

    public void OnAbilityUpgraded(UpgradeItemVisual item)
    {
        // returns true if bought successfully 
        var ability = inventoryControl.abilities.Find(x => x.ability == item.UpgradeAbility);
        ability.AddLevels(1);

        isUpgradesOpen = false;
        CloseUpgrades();
    }

    private void ScaleUpgradeUI()
    {
        // update previous screen size
        previousScreenSize = new Vector2(shopCanvas.pixelRect.width, shopCanvas.pixelRect.height);
        sizeCoeficient = new Vector2(previousScreenSize.x / defaultSize.x, previousScreenSize.y / defaultSize.y);

        backgroundImage.rectTransform.sizeDelta = new(defaultBackgroundRect.width * sizeCoeficient.x, defaultBackgroundRect.height * sizeCoeficient.y);
        backgroundImage.rectTransform.anchoredPosition = new Vector3(anchoredBackgroundPosition.x * sizeCoeficient.x, anchoredBackgroundPosition.y * sizeCoeficient.y, 0);

        scrollbarMask.softness = new((int)(defaultMaskSoftness.x * sizeCoeficient.x), (int)(defaultMaskSoftness.y * sizeCoeficient.y));
        layout.spacing = defaultShopItemSpacing * sizeCoeficient.y;
        layout.padding = new((int)(defaultLayoutOffset.left * sizeCoeficient.x), (int)(defaultLayoutOffset.right * sizeCoeficient.x), (int)(defaultLayoutOffset.top * sizeCoeficient.y), (int)(defaultLayoutOffset.bottom * sizeCoeficient.y));


        upgradeText.fontSize = defaultUpgradeTextsize * ((sizeCoeficient.x + sizeCoeficient.y) / 2);
        upgradeText.rectTransform.anchoredPosition = new(defaultUpgradeTextPosition.x * sizeCoeficient.x, defaultUpgradeTextPosition.y * sizeCoeficient.y);

        scrollbar.anchoredPosition = new Vector2(defaultScrollPosition.x * sizeCoeficient.x, defaultScrollPosition.y * sizeCoeficient.y);
        scrollbar.sizeDelta = new Vector2(defaultScrollSize.x * sizeCoeficient.x, defaultScrollSize.y * sizeCoeficient.y);
        upgradeItems.ForEach(item => item.UpdateScales(sizeCoeficient));
        UpdateItemHolderHeight();
    }


    private void SetupShopItems(InventoryControl inventory)
    {
        foreach (var ability in inventory.abilities)
        {
            if (!ability.hasLevels || ability.Level >= ability.maxLevel)
                continue;
            var item = Instantiate(itemPrefab, shopItemHolder);
            var visual = item.GetComponent<UpgradeItemVisual>();
            visual.SetupItem(this, ability);
            upgradeItems.Add(visual);

        }

        UpdateItemHolderHeight();
    }

    private void UpdateItemHolderHeight()
    {
        if (upgradeItems.Count > 0)
            shopItemHolder.sizeDelta = new(shopItemHolder.sizeDelta.x, (layout.spacing + layout.padding.top + upgradeItems[0].background.rectTransform.sizeDelta.y) * upgradeItems.Count);
    }

    public void OpenUpgrades()
    {
        if (isUpgradesOpen)
            return;
        // if is closed and other window open return
        if (inventoryControl.WindowOpen)
            return;

        if (player)
            player.DisablePlayerControls(true);

        isUpgradesOpen = true;
        inventoryControl.WindowOpen = true;
        ClearUpgradeItems();
        SetupShopItems(inventoryControl);
        backgroundImage.gameObject.SetActive(true);
    }
    public void CloseUpgrades()
    {
        if (!isUpgradesOpen)
            return;

        if (player)
            player.DisablePlayerControls(true);

        isUpgradesOpen = false;
        inventoryControl.WindowOpen = false;
        ClearUpgradeItems();
        backgroundImage.gameObject.SetActive(false);
    }

    private void ClearUpgradeItems()
    {
        foreach (var item in upgradeItems)
        {
            Destroy(item.gameObject);
        }
        upgradeItems.Clear();
    }

    public void ChangeUpgradeState()
    {
        // Add a price to ability upgrades

        if (isUpgradesOpen)
            OpenUpgrades();
        else
            CloseUpgrades();
    }

    public void OnCloseUIKey(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        CloseUpgrades();
    }
}
