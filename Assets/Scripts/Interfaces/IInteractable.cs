using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    // If a text is show, it will always disapear when the object is destroyed

    bool LockPlayerControls { get; }
    bool IsInteractable { get; }

    void PlayerEnter();
    void PlayerExit();

    void Interact();
}
