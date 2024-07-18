using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    private PlayerInput input;


    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.0f;
    private InputAction movementAction;


    [Header("Animation")]
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        input = GetComponent<PlayerInput>();
        //input.actions["Move"].performed += Movement;
        //input.actions["Fire"].performed += Attack;
        
        movementAction = input.actions["Move"];
    }

    // Player actions are bound in Inspector (Player Input -> Behavior -> Invoke Unity Events) 

    // Todo:
    // Movement Animation Bind to direction (pass the movement direction to the animator)
    /*
    public void Movement(InputAction.CallbackContext obj)
    {
        var direction = obj.ReadValue<Vector2>().normalized;
        print($"I Moved {direction}");
    }
    */
    private void Movement()
    {
        var direction = movementAction.ReadValue<Vector2>().normalized;
        transform.position = transform.position + Utils.GetVec3(direction) * moveSpeed * Time.deltaTime;

        animator.SetFloat("directionX", direction.x);
        animator.SetFloat("directionY", direction.y);
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
        // As PlayerInput events are not called every frame we check the value our selves
        Movement();
    }

}
