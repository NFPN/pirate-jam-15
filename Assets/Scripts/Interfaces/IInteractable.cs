using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    bool LockPlayerControls { get; }
    bool IsInteractable { get; }

    void PlayerEnter();
    void PlayerExit();

    void Interact();
}
