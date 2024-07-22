using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientRune : MonoBehaviour, IInteractable
{

    private Vector2 spriteSize;
    //private bool inRange = false;


    public bool isInteractable;
    private bool textShown = false;

    public bool LockPlayerControls => false;
    public bool IsInteractable => isInteractable;

    public void Interact()
    {
        isInteractable= false;
        textShown = true;
        KeyIndicatorControl.inst.HideIndicator();
        TextSystem.inst.DisplayText(transform, new Vector2(0, 0.5f), "AncientRuneEarth", 0);
    }

    public void PlayerEnter()
    {
       //inRange = true;
       KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, gameObject, new Vector2(spriteSize.x/2, 0));
    }

    public void PlayerExit()
    {
        //inRange = false;
        KeyIndicatorControl.inst.HideIndicator();
        if (textShown)
            TextSystem.inst.HideText();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
        TextSystem.inst.OnTextHidden += (source) =>
        {
            Destroy(gameObject);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
