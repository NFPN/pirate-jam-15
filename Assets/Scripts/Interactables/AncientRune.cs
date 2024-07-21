using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientRune : MonoBehaviour, IInteractable
{

    private Vector2 spriteSize;
    private bool inRange = false;


    public bool isInteractable;

    public bool LockPlayerControls => false;
    public bool IsInteractable => isInteractable;

    public void Interact()
    {
        //isInteractable= false;
        //KeyIndicatorControl.inst.HideIndicator();
        TextSystem.inst.DisplayText(transform, new Vector2(0, 0.5f), "AncientRuneEarth", 0);
    }

    public void PlayerEnter()
    {
       inRange = true;
       KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, gameObject, new Vector2(spriteSize.x/2, 0));
    }

    public void PlayerExit()
    {
        inRange = false;
        KeyIndicatorControl.inst.HideIndicator();
        TextSystem.inst.HideText();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
        TextSystem.inst.OnTextHidden += (source) =>
        {
            if (source == transform && inRange)
                PlayerEnter();
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
