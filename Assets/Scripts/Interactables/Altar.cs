using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour, IInteractable
{

    public bool LockPlayerControls => lockPlayerControls;

    public bool IsInteractable => isInteractable;


    public Utils.AltarType altarType;


    public bool lockPlayerControls;
    private bool isInteractable = true;

    public GameObject indicatorLocation;



    public void Interact()
    {
        if (altarType == Utils.AltarType.Health)
            HealthInteract();
        else if (altarType == Utils.AltarType.Upgrade)
            UpgradeInteract();
    }

    private void HealthInteract()
    {

    }

    private void UpgradeInteract()
    {

    }



    public void PlayerEnter()
    {
        KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, indicatorLocation, Vector2.zero);
    }

    public void PlayerExit()
    {
        throw new System.NotImplementedException();
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
