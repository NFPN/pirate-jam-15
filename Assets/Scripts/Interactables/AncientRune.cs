using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncientRune : MonoBehaviour, IInteractable
{

    private Vector2 spriteSize;


    public bool isInteractable;

    public bool LockPlayerControls => false;
    public bool IsInteractable => isInteractable;

    public void Interact()
    {
        isInteractable= false;
        KeyIndicatorControl.inst.HideIndicator();
        TextSystem.inst.DisplayText(transform, new Vector2(0, 0.5f), "AncientRuneEarth", 0);
    }

    public void PlayerEnter()
    {
       KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, gameObject, new Vector2(spriteSize.x/2, 0));
    }

    public void PlayerExit()
    {
        KeyIndicatorControl.inst.HideIndicator();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteSize = GetComponent<SpriteRenderer>().size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
