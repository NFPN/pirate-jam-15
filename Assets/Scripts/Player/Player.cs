using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IHealth
{
    public event IHealth.HealthChangedHandler OnHealthChanged;

    public event IHealth.DeathHandler OnDeath;

    public ObjectPooler ObjectPooler;

    private SpriteRenderer spriteRenderer;
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public new Rigidbody2D rigidbody2D;
    [HideInInspector] public Vector2 directionVector;
    [HideInInspector] public Vector2 lastDirectionVector = Vector2.down;

    public PlayerStateMachine StateMachine { get; set; }
    public PlayerJumpState JumpState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    public PlayerDashState DashState { get; set; }
    public bool IsControlable { get => isControlable; }

    [Header("Health")]
    [SerializeField] private float maxHealth;

    [SerializeField] private float knockbackForce;
    public float knockbackDistance = 1;
    private float health;

    public float CurrentHealth => health;
    public float MaxHealth => DataControl.inst.playerHealthMax;

    [Header("Movement")]
    public float moveSpeed = 1.0f;

    public float jumpForce = 20.0f;
    public float dashSpeed = 5.0f;
    public float dashDuration = 1.0f;

    public List<DashLevel> dashLevels;
    public DashLevel CurrentDashLevel { get; private set; }

    [Header("Animation")]
    public Animator animator;

    [HideInInspector] public Utils.Direction currentDirection = Utils.Direction.Right;
    [HideInInspector] public bool isKnockback = false;

    public float knockBackStopTime = 0.1f;
    private float knockBackAppliedTime;

    // Disables or enables all player movement
    private bool isControlable = true;

    private bool canAttack = true;
    private bool isAttacking;

    private DataControl persistentData;
    private InputControl inputControl;

    private InventoryControl inventory;

    public MeleeRing meleeRing;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        StateMachine = new PlayerStateMachine();

        //JumpState = new PlayerJumpState(this, StateMachine);
    }

    private void SubscribeToInputEvents()
    {
        inputControl.Subscribe("Move", OnMove);
        inputControl.Subscribe("Attack", Attack);
        inputControl.Subscribe("Dash", OnDash);
        inputControl.Subscribe("SecondaryAttack", SecondaryAttack);
    }

    private void OnDisable()
    {
        inputControl.Unsubscribe("Move", OnMove);
        inputControl.Unsubscribe("Attack", Attack);
        inputControl.Unsubscribe("Dash", OnDash);
        inputControl.Unsubscribe("SecondaryAttack", SecondaryAttack);
    }

    private void Start()
    {
        persistentData = DataControl.inst;
        inventory = InventoryControl.inst;
        inputControl = InputControl.inst;
        input = inputControl.input;

        SubscribeToInputEvents();

        MoveState = new PlayerMoveState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);

        StateMachine.Initialize(MoveState);

        // For no dependency on world change controller
        if (WorldShaderControl.inst != null)
        {
            WorldShaderControl.inst.OnChangeSpriteVisual += OnChangeToShadow;
            WorldShaderControl.inst.OnUpdateIsPlayerControllable += OnUpdateWorldShader;
            OnChangeToShadow(WorldShaderControl.inst.IsShadowWorld);
        }

        SetHealth(persistentData.GetCurrentPlayerHealth());
    }

    private void OnUpdateWorldShader()
    {
        WorldShaderControl.inst.UpdatePlayerControllable(isControlable);
    }

    private void OnChangeToShadow(bool isShadow)
    {
        // change which blend tree we are using
        animator.SetFloat("blendIndex", isShadow ? 1 : 0);
    }

    public void Attack(InputAction.CallbackContext obj)
    {
        if (!canAttack || !isControlable || obj.phase != InputActionPhase.Started)
            return;

        var fireballData = inventory.GetAbilityData(Utils.Abilities.Fireball);
        if (fireballData.IsLocked)
            return;

        // fireballData.level
        // Do stat update based on level
        animator.Play("Cast");
    }

    public void CastFireball()
    {
        if (isAttacking)
            return;

        StartCoroutine(Fireball());
    }

    //TODO:We should improve this later
    private IEnumerator Fireball()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        var fireball = ObjectPooler.GetObj("ShadowFireball");

        if (fireball != null)
        {
            AudioControl.inst.PlayOneShot(Utils.SoundType.Fireball);

            var proj = fireball.GetComponent<Projectile>();

            proj.SetDirection(lastDirectionVector);

            proj.SetProjectileStats(inventory.GetAbilityData(Utils.Abilities.Fireball).Level);
            //TODO: use damage from player upgrade system

            fireball.transform.SetPositionAndRotation(
                transform.position + lastDirectionVector.ToVector3() * 0.7f,
                Quaternion.FromToRotation(Vector3.right, lastDirectionVector));
            fireball.SetActive(true);

            yield return new WaitForSeconds(proj.CurrentLevel.castDelay);
        }
        isAttacking = false;
    }

    public void SecondaryAttack(InputAction.CallbackContext obj)
    {
        if (!isControlable)
            return;

        if (obj.phase != InputActionPhase.Started)
            return;

        var aoeData = inventory.GetAbilityData(Utils.Abilities.AOEMagic);
        if (aoeData.IsLocked)
            return;

        // aoeData.level
        // Do stat update based on level
        if (meleeRing != null)
            meleeRing.AOEAttack(aoeData.Level);
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        directionVector = obj.ReadValue<Vector2>();

        if (!isControlable)
            rigidbody2D.velocity = Vector2.zero;

        if (directionVector != Vector2.zero && isControlable)
        {
            lastDirectionVector = obj.ReadValue<Vector2>();

            // Pass the player facing to the animator
            animator.SetFloat("rotationX", lastDirectionVector.x);
            animator.SetFloat("rotationY", lastDirectionVector.y);

            AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.IsWalking, 1);
        }
        else
            AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.IsWalking, 0);
    }

    public void OnDash(InputAction.CallbackContext obj)
    {
        if (!isControlable)
            return;

        if (StateMachine.CurrentState == DashState || obj.phase != InputActionPhase.Started)
            return;

        var dashData = inventory.GetAbilityData(Utils.Abilities.Dash);
        if (dashData.IsLocked)
            return;

        // dashData.level <- take from here
        //TODO: update dash data based on level
        CurrentDashLevel = dashLevels[dashData.Level - 1];

        StateMachine.ChangeState(DashState);
    }

    private void Update()
    {
        // As PlayerInput events are not called every frame we check the value our selves
        StateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        if (isControlable)
            StateMachine.CurrentState.PhysicsUpdate();
    }

    public void DealDamage(object source, float damage)
    {
        if (StateMachine.CurrentState is PlayerDashState && CurrentDashLevel.dashImunity)
            return;

        if (source is Enemy)
        {
            var gameObj = (Enemy)source;
            StartCoroutine(ApplyKnockback(-(gameObj.transform.position - transform.position).normalized));
        }

        var oldHealth = health;
        health -= damage;
        health = Mathf.Clamp(health, 0, persistentData.playerHealthMax);

        if (damage > 0)
            AudioControl.inst.PlayOneShot(Utils.SoundType.PlayerHit);

        InvokeHealthEvents(source, oldHealth, health);
    }

    public void SetHealth(float amount)
    {
        var oldHealth = health;
        health = amount;
        health = Mathf.Clamp(health, 0, persistentData.playerHealthMax);

        InvokeHealthEvents(this, oldHealth, health);
    }

    private void InvokeHealthEvents(object sender, float oldHealth, float newHealth)
    {
        OnHealthChanged?.Invoke(sender, oldHealth, newHealth);
        if (newHealth == 0)
            OnDeath?.Invoke(sender);
    }

    /// Interaction Logic

    public void DisableAttack(bool state) => canAttack = !state;

    public void DisablePlayerControls(bool state)
    {
        isControlable = !state;
        rigidbody2D.velocity = Vector2.zero;
        animator.SetFloat("directionX", 0);
        animator.SetFloat("directionY", 0);
    }

    private IEnumerator ApplyKnockback(Vector3 knockbackDir)
    {
        isKnockback = true;
        var startPos = transform.position;
        knockBackAppliedTime = Time.time;

        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        while (Vector3.Distance(startPos, transform.position) < knockbackDistance && knockBackAppliedTime + knockBackStopTime > Time.time)
        {
            yield return new WaitForSeconds(0.01f);
        }
        rigidbody2D.velocity = Vector2.zero;
        isKnockback = false;
    }

    public void Teleport(Vector3 location)
    {
        rigidbody2D.isKinematic = true;
        transform.position = location;
        rigidbody2D.isKinematic = false;
        rigidbody2D.velocity = Vector2.zero;
        Camera.main.transform.position = new Vector3(location.x, location.y, Camera.main.transform.position.z);
    }
}

[System.Serializable]
public struct DashLevel
{
    public float dashSpeed;
    public float dashDuration;
    public float cooldown;
    public bool dashImunity;
}