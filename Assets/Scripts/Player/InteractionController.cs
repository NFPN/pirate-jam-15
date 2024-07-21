using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    private bool canInteract = true;

    private Player player;

    // Interactables in range
    private Dictionary<Transform, IInteractable> interactables = new();

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
            Destroy(this); 

        // Interaction disabled during world switch

        WorldShaderControl.inst.OnWorldChangeBegin += (isShadow) => canInteract = false;
        WorldShaderControl.inst.OnWorldChangeComplete += () => canInteract = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnInteractionKeyPress(InputAction.CallbackContext callbackContext)
    {
        if(callbackContext.phase == InputActionPhase.Started && canInteract)
        {
            InteractWithClosestObject();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!collision.gameObject.TryGetComponent<IInteractable>(out var interactable))
            return;

        if (interactable.IsInteractable)
            interactable.PlayerEnter();

        if(!interactables.ContainsKey(collision.transform))
        interactables.Add(collision.transform, interactable);
        player.DisableAttack(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!interactables.ContainsKey(collision.transform))
            return;

        interactables[collision.transform].PlayerExit();
        interactables.Remove(collision.transform);
        if (interactables.Count == 0)
            player.DisableAttack(false);
    }

    private void InteractWithClosestObject()
    {
        if(!canInteract)
            return;

        if(interactables.Count == 0) return;

        var closestTransform = interactables.Keys.OrderBy(x => Vector3.Distance(transform.position, x.position)).First();
        var closesetInteractable = interactables[closestTransform];

        if (closesetInteractable.IsInteractable)
            closesetInteractable.Interact();
    }
}
