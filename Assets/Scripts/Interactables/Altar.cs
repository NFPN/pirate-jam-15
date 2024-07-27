using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{

    public bool LockPlayerControls => lockPlayerControls;
    public bool IsInteractable => isInteractable;

    //public Utils.AltarType altarType;
    public bool lockPlayerControls;
    public float destructionChance = 0.5f;
    public Sprite destroyedAltar;

    private bool isInteractable = true;



    public GameObject indicatorLocation;
    public GameObject textPosition;



    public void Interact()
    {
        if (Random.Range(0.0f, 1.0f) < destructionChance)
        {
            isInteractable = false;
            TextSystem.inst.DisplayText(textPosition, Vector2.zero, "Altar", 4);
            KeyIndicatorControl.inst.HideIndicator();
            GetComponent<SpriteRenderer>().sprite = destroyedAltar;

            DataControl.inst.AddUsedObject(gameObject);
            return;
        }

        var heldItem = UIHeldItem.inst.GetCurrentHeldItem();

        if (heldItem == null)
        {
            TextSystem.inst.DisplayText(textPosition, Vector2.zero, "Altar", 0);
            return;
        }

        if (heldItem.OwnedCount == 0)
        {
            TextSystem.inst.DisplayText(textPosition, Vector2.zero, "Altar", 1);
            return;
        }

        if (heldItem.item == Utils.Items.Heart)
        {
            TextSystem.inst.DisplayText(textPosition, Vector2.zero, "Altar", 2);
            HealthInteract();
        }
        else if (heldItem.item == Utils.Items.AncientRune)
        {
            TextSystem.inst.DisplayText(textPosition, Vector2.zero, "Altar", 3);
            UpgradeInteract();
        }
        else
        {
            TextSystem.inst.DisplayText(textPosition, Vector2.zero, "Altar", 1);
        }


    }

    private void HealthInteract()
    {
        var player = FindObjectOfType<Player>();
        if (!player)
            return;

        if (InventoryControl.inst)
            InventoryControl.inst.UseItem(Utils.Items.Heart);

        if (player.CurrentHealth == player.MaxHealth)
        {
            DataControl.inst.AddPlayerMaxHealth(2);
        }
        player.DealDamage(this, -2);

    }

    private void UpgradeInteract()
    {
        if (InventoryControl.inst)
        {
            InventoryControl.inst.UseItem(Utils.Items.AncientRune);
            InventoryControl.inst.OpenUpgrades();
        }
    }



    public void PlayerEnter()
    {
        KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, indicatorLocation, Vector2.zero);
    }

    public void PlayerExit()
    {
        KeyIndicatorControl.inst.HideIndicator();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
