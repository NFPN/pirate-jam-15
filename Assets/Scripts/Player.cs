using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    private PlayerInput input;

    // Start is called before the first frame update
    private void Start()
    {
        //input = GetComponent<PlayerInput>();
        //input.actions["Move"].performed += Movement;
        //input.actions["Fire"].performed += Attack;
    }

    public void Movement(InputAction.CallbackContext obj)
    {
        var direction = obj.ReadValue<Vector2>();
        print($"I Moved {direction}");
    }

    public void Attack(InputAction.CallbackContext obj)
    {
        print($"I attacked {obj.action}");
    }

    public void OnMove(InputValue dir)
    {
        print($"I Moved {dir.Get<Vector2>()}");
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
