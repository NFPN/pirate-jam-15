using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [HideInInspector] public Vector2 lastDirectionVector;

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

    [Header("Animation")]
    public Animator animator;


    [HideInInspector] public Utils.Direction currentDirection = Utils.Direction.Right;
    [HideInInspector] public bool isKnockback = false;


    // Disables or enables all player movement
    private bool isControlable = true;

    private bool canAttack = true;
    private bool isAttacking;
    public GameObject AoeCollision;

    private DataControl persistentData;
    private InputControl inputControl;

    private InventoryControl inventory;


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


        AoeCollision.SetActive(false);
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

        StartCoroutine(Fireball());

    }

    //TODO:We should improve this later
    private IEnumerator Fireball()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        var fireball = ObjectPooler.GetObj("ShadowFireball");

        if (fireball != null)
        {
            var proj = fireball.GetComponent<Projectile>();

            proj.SetDirection(lastDirectionVector);
            proj.damage = 1;//TODO: use damage from player upgrade system

            fireball.transform.SetPositionAndRotation(
                transform.position + lastDirectionVector.ToVector3() * 0.7f,
                Quaternion.FromToRotation(Vector3.right, lastDirectionVector));
            fireball.SetActive(true);
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

        StartCoroutine(AOEMagic());

    }

    private IEnumerator AOEMagic()
    {
        if (isAttacking) yield break;

        isAttacking = true;
        AoeCollision.SetActive(true);

        //TODO: Add settings to class
        var circleCollider = AoeCollision.GetComponent<CircleCollider2D>();
        var startRadius = .5f;
        var duration = .2f;
        var targetRadius = 2f;
        var elapsedTime = 0f;

        circleCollider.radius = startRadius;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            circleCollider.radius = Mathf.Lerp(startRadius, targetRadius, elapsedTime / duration);
            yield return null;
        }

        circleCollider.radius = targetRadius;
        AoeCollision.SetActive(false);
        isAttacking = false;
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        directionVector = obj.ReadValue<Vector2>();

        if (directionVector != Vector2.zero && isControlable)
        {
            lastDirectionVector = obj.ReadValue<Vector2>();

            // Pass the player facing to the animator
            animator.SetFloat("rotationX", lastDirectionVector.x);
            animator.SetFloat("rotationY", lastDirectionVector.y);

        }
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
        if (StateMachine.CurrentState is PlayerDashState)
            return;

        if(source is Enemy)
        {
            var gameObj = (Enemy)source;
            StartCoroutine(ApplyKnockback(-(gameObj.transform.position - transform.position).normalized));
        }

        var oldHealth = health;
        health -= damage;
        health = Mathf.Clamp(health, 0, persistentData.playerHealthMax);
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

    public void DisablePlayerControls(bool state) => isControlable = !state;

    private IEnumerator ApplyKnockback(Vector3 knockbackDir)
    {
        isKnockback = true;
        var startPos = transform.position;

        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        while(Vector3.Distance(startPos,transform.position) < knockbackDistance)
        {
            yield return new WaitForSeconds(0.01f);
        }
        rigidbody2D.velocity = Vector2.zero;
        isKnockback = false;

    }

}
