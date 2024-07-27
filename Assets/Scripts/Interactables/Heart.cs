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

    public int maxTextIndex = 3;
    private int textIndex = 0;
    private bool forceExited = false;

    public void Interact()
    {
        if (textIndex == maxTextIndex)
        {
            isInteractable = false;
            KeyIndicatorControl.inst.HideIndicator();
        }
        textShown = true;
        TextSystem.inst.DisplayText(gameObject, new Vector2(0, 0.5f), "Heart", textIndex++, OnTextHidden);
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
            forceExited = true;
            TextSystem.inst.HideText();
            if(textIndex == maxTextIndex -1)
            {
                Destroy(gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
    }

    private void OnTextHidden()
    {
        if (textIndex == maxTextIndex && !forceExited)
        {
            InventoryControl.inst.AddItem(Utils.Items.Heart);
            DataControl.inst.AddUsedObject(gameObject);
            Destroy(gameObject);
        }


    }

}
