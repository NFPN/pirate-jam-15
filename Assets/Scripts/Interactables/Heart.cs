using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour, IInteractable
{
    private Vector2 spriteSize;
    //private bool inRange = false;


    public bool isInteractable;
    private bool textShown = false;

    public bool LockPlayerControls => false;
    public bool IsInteractable => isInteractable;

    public string textName = "Heart";
    public List<int> textIndexes = new List<int>() { 0 };
    private int textIndex = 0;

    public void Interact()
    {
        if (textIndex == textIndexes.Count -1)
        {
            isInteractable = false;
            KeyIndicatorControl.inst.HideIndicator();
        }
        textShown = true;
        TextSystem.inst.DisplayText(gameObject, new Vector2(0, 0.5f), textName, textIndexes[textIndex++], OnTextHidden);
    }

    public void PlayerEnter()
    {
        //inRange = true;
        KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, gameObject, new Vector2(spriteSize.x / 2, 0));
    }

    public void PlayerExit()
    {
        //inRange = false;
        KeyIndicatorControl.inst.HideIndicator();
        if (textShown)
        {
            TextSystem.inst.HideText();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
    }

    private void OnTextHidden()
    {
        if (textIndex == textIndexes.Count)
        {
            InventoryControl.inst.AddItem(Utils.Items.Heart);
            DataControl.inst.AddUsedObject(gameObject);
            Destroy(gameObject);
        }


    }

}
