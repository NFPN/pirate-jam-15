using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    PlayerInput input;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInput>();
        foreach (var action in input.actions)
        {
            if (action.name == "Move")
            {
                print("found move");
                action.performed += Action_performed;
            }
        }
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        print("I was performed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
