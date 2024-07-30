using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IHealth
{
    [Header("Health")]
    [SerializeField] protected float maxHealth;

    [Header("Movement")]
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float minDistanceToPlayer;
    [SerializeField] protected bool followPlayer;
    [SerializeField] protected PlayerDetector playerDetection;
    [SerializeField] protected PlayerDetector attackRange;

    [Header("Attack")]
    [SerializeField] protected int damage;

    [Header("Drops")]
    [SerializeField] protected List<ItemDrop> drops;
    [SerializeField] protected Vector2 dropArea;


    [Header("Persistence (false -> Object Save Data Required)")]
    [SerializeField] protected bool respawnNextEnter = false;


    protected bool HasKnockbackAnim { get; set; }
    protected bool HasAttackAnim { get; set; }
    protected bool IsDead { get; set; }

    public float CurrentHealth { get; set; }

    public float MaxHealth => maxHealth;

    public event IHealth.HealthChangedHandler OnHealthChanged;

    public event IHealth.DeathHandler OnDeath;

    protected bool canMove = true;
    protected bool animationDamageDealt = false;
    protected bool isAttacking = false;

    protected Player player;
    protected Animator animator;
    protected NavMeshAgent navAgent;



    protected virtual void Awake()
    {
        InitializeEnemy();
    }

    protected void InitializeEnemy()
    {
        CurrentHealth = MaxHealth;
        player = FindAnyObjectByType<Player>();
        if (player == null)
            print("Player not found");

        animator = GetComponent<Animator>();


        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.speed = movementSpeed;
    }


    public void DealDamage(object source, float damage)
    {
        var oldCurrentHealth = CurrentHealth;
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        InvokeCurrentHealthEvents(source, oldCurrentHealth, CurrentHealth);

        KnockbackAnimation();
    }

    public void SetHealth(float amount)
    {
        var oldCurrentHealth = CurrentHealth;
        CurrentHealth = amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        InvokeCurrentHealthEvents(this, oldCurrentHealth, CurrentHealth);
    }

    private void InvokeCurrentHealthEvents(object sender, float oldCurrentHealth, float newCurrentHealth)
    {
        OnHealthChanged?.Invoke(sender, oldCurrentHealth, newCurrentHealth);
        if (newCurrentHealth == 0)
            OnDeath?.Invoke(sender);
    }

    protected virtual void SetDestination(Vector3 position)
    {
        if (canMove)
        {
            navAgent.SetDestination(position);
        }
    }

    protected virtual void EnemyDeath(object source)
    {
        canMove = false;
        IsDead = true;

        StopMovement();
        if (playerDetection)
            Destroy(playerDetection);
        if (attackRange)
            Destroy(attackRange);

        if (!respawnNextEnter && DataControl.inst)
            DataControl.inst.AddUsedObject(gameObject);

        animator.Play("Death");
    }

    protected virtual void KnockbackAnimation()
    {
        if (HasKnockbackAnim && !IsDead)
        {
            StopMovement();
            animator.Play("Knockback");
        }
    }
    protected virtual void AttackAnimation()
    {
        if (HasAttackAnim && !IsDead)
        {
            isAttacking = true;
            StopMovement();
            animator.Play("Attack");
        }
    }

    protected void StopMovement()
    {
        if (!isAttacking)
            navAgent.isStopped = true;
    }

    protected void DoMovement()
    {
        navAgent.isStopped = false;
    }

    public virtual void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public virtual void DropItems()
    {
        foreach (var item in drops)
        {
            for (int i = 0; i < item.count; i++)
            {
                var pos = transform.position;
                pos.x += Random.Range(-dropArea.x, dropArea.x);
                pos.y += Random.Range(-dropArea.y, dropArea.y);
                Instantiate(item.itemPrefab, pos, Quaternion.Euler(Utils.GetRandomRotationZ()));
            }
        }
    }

    public virtual void DealDamageToPlayer()
    {
        if (animationDamageDealt)
            return;
        animationDamageDealt = true;

        player.DealDamage(this, damage);

        isAttacking = false;
    }
}
