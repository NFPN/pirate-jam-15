using TMPro.EditorUtilities;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput),typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    [HideInInspector]
    public PlayerInput input;
    public SpriteRenderer spriteRenderer;


    public PlayerStateMachine StateMachine { get; set; }

    public PlayerJumpState JumpState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    public PlayerDashState DashState { get; set; }

    [Header("Movement")]
    public float moveSpeed = 1.0f;
    public float jumpForce = 20.0f;
    public float dashSpeed = 5.0f;
    public float dashDuration = 1.0f;


    [Header("Animation")]
    public Animator animator;
    [HideInInspector] public Utils.Direction currentDirection = Utils.Direction.Right;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StateMachine = new PlayerStateMachine();

        JumpState = new PlayerJumpState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);

    }

    // Start is called before the first frame update
    private void Start()
    {
        StateMachine.Initialize(MoveState);
        //input.actions["Move"].performed += Movement;
        //input.actions["Fire"].performed += Attack;
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

    public void Attack(InputAction.CallbackContext obj)
    {
        print($"I attacked {obj.action}");
    }

    public void OnMove(InputValue dir)
    {
        print($"I Moved {dir.Get<Vector2>()}");
    }

    public void OnJump(InputAction.CallbackContext obj)
    {
        if (StateMachine.CurrentState != JumpState)
            StateMachine.ChangeState(JumpState);
    }
    
    public void OnDash(InputAction.CallbackContext obj)
    {
        if(StateMachine.CurrentState != DashState)
            StateMachine.ChangeState(DashState);
    }

    public void UpdatePlayerDirection(Utils.Direction direction)
    { 
        currentDirection = direction;
        spriteRenderer.flipX = Utils.Direction.Left == direction;
    }

    // Update is called once per frame
    private void Update()
    {
        // As PlayerInput events are not called every frame we check the value our selves
        StateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
}
