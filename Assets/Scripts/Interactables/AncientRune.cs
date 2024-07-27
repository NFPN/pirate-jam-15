using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientRune : MonoBehaviour, IInteractable
{
    public GameObject textLocation;
    public GameObject keyLocation;

    private Vector2 spriteSize;
    //private bool inRange = false;


    public bool isInteractable;
    private bool textShown = false;

    public bool LockPlayerControls => false;
    public bool IsInteractable => isInteractable;

    public bool unlocksAbility = false;
    public Utils.Abilities abilityType;

    public List<int> textIndexes = new List<int>() { 0 };

    private int currentIndex = 0;

    public void Interact()
    {
        isInteractable = false;
        textShown = true;
        KeyIndicatorControl.inst.HideIndicator();
        TextSystem.inst.DisplayText(textLocation, Vector2.zero, "Rune", textIndexes[currentIndex], OnTextHidden);
    }

    public void PlayerEnter()
    {
        //inRange = true;
        KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, keyLocation, Vector2.zero);
    }

    public void PlayerExit()
    {
        //inRange = false;
        KeyIndicatorControl.inst.HideIndicator();
        if (textShown)
        {
            TextSystem.inst.HideText();
            textShown = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
    }


    private void OnTextHidden()
    {
        if (currentIndex == textIndexes.Count - 1)
        {
            if (!unlocksAbility)
                InventoryControl.inst.AddItem(Utils.Items.AncientRune);
            else
                InventoryControl.inst.UnlockAbility(abilityType);
            DataControl.inst.AddUsedObject(gameObject);
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
