using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IHealth
{
    public event IHealth.HealthChangedHandler OnHealthChanged;

    public event IHealth.DeathHandler OnDeath;

    public ObjectPooler ObjectPooler;

    private SpriteRenderer spriteRenderer;
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public new Rigidbody2D rigidbody2D;
    [HideInInspector] public Vector2 directionVector;
    [HideInInspector] public Vector2 lastDirectionVector;

    public PlayerStateMachine StateMachine { get; set; }
    public PlayerJumpState JumpState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    public PlayerDashState DashState { get; set; }

    [Header("Health")]
    [SerializeField] private float maxHealth;

    private float health;

    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

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
        rigidbody2D = GetComponent<Rigidbody2D>();
        StateMachine = new PlayerStateMachine();

        //JumpState = new PlayerJumpState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(MoveState);
        WorldShaderControl.inst.OnChangeSpriteVisual += OnChangeToShadow;

        SetHealth(maxHealth);
    }

    private void OnChangeToShadow(bool isShadow)
    {
        // change which blend tree we are using
        animator.SetFloat("blendIndex", isShadow ? 1 : 0);
    }

    public void Attack(InputAction.CallbackContext obj)
    {
        if (obj.phase == InputActionPhase.Started)
        {
            //animator.setTrigger("attack");

            var fireball = ObjectPooler.GetObj("ShadowFireball");

            if (fireball != null)
            {
                var proj = fireball.GetComponent<Projectile>();
                proj.SetDirection(lastDirectionVector);

                fireball.transform.SetPositionAndRotation(
                    transform.position + lastDirectionVector.ToVector3() * 0.7f,
                    Quaternion.FromToRotation(Vector3.right, lastDirectionVector));
            }
        }
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        directionVector = obj.ReadValue<Vector2>();

        if (directionVector != Vector2.zero)
            lastDirectionVector = obj.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext obj)
    {
        if (StateMachine.CurrentState != DashState && obj.phase == InputActionPhase.Started)
            StateMachine.ChangeState(DashState);
    }

    public void UpdatePlayerDirection(Utils.Direction direction)
    {
        currentDirection = direction;
        spriteRenderer.flipX = Utils.Direction.Left == direction;
    }

    private void Update()
    {
        // As PlayerInput events are not called every frame we check the value our selves
        StateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    public void DealDamage(object source, float damage)
    {
        if (StateMachine.CurrentState is PlayerDashState)
            return;

        var oldHealth = health;
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        InvokeHealthEvents(source, oldHealth, health);
        print(health);
    }

    public void SetHealth(float amount)
    {
        var oldHealth = health;
        health = amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        InvokeHealthEvents(this, oldHealth, health);
    }

    private void InvokeHealthEvents(object sender, float oldHealth, float newHealth)
    {
        OnHealthChanged?.Invoke(sender, oldHealth, newHealth);
        if (newHealth == 0)
            OnDeath?.Invoke(sender);
    }
}
