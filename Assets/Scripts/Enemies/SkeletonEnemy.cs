using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : Enemy
{
    private bool playerInRange;
    private bool inAttackRange;

    public float attackSpeed;
    private float lastAttackTime;

    public SpriteRenderer enemyVisual;

    private bool inMinDist;

    private Vector2 direction;

    private void Start()
    {
        OnDeath += EnemyDeath;
        OnDeath += (source) => AudioControl.inst.PlayOneShot(Utils.SoundType.SkellyDeath);
        OnHealthChanged += (source, old, newHealth) => { if (newHealth < old) { AudioControl.inst.PlayOneShot(Utils.SoundType.SkellyHit); } };

        playerDetection.OnPlayerInRange += () => { playerInRange = true; DoMovement(); };
        playerDetection.OnPlayerOutOfRange += () => { playerInRange = false; StopMovement(); };

        attackRange.OnPlayerInRange += () => inAttackRange = true;
        attackRange.OnPlayerOutOfRange += () => inAttackRange = false;

        HasAttackAnim = true;
        //HasKnockbackAnim = true;
    }

    private void Update()
    {
        UpdateSlimeFacing();
        if (followPlayer)
        {
            FollowPlayer();
        }
        Attack();

        navAgent.speed = movementSpeed;
    }

    private void FollowPlayer()
    {
        if (!playerInRange)
            return;

        if (Vector3.Distance(transform.position, player.transform.position) < minDistanceToPlayer && !inMinDist)
        {
            StopMovement();
            inMinDist = true;
        }
        if (Vector3.Distance(transform.position, player.transform.position) > minDistanceToPlayer && inMinDist)
        {
            DoMovement();
            inMinDist = false;
        }

        else
        {
            SetDestination(player.transform.position);
        }
    }

    private void Attack()
    {
        if (attackSpeed + lastAttackTime > Time.time || !inAttackRange)
            return;
        lastAttackTime = Time.time;
        animationDamageDealt = false;
        AttackAnimation();
    }

    private void UpdateSlimeFacing()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 pos = transform.position;
        var rot = (playerPos - pos).normalized;
        if (rot != Vector2.zero)
            direction = rot;
        if (!playerInRange)
            rot = Vector2.zero;
        animator.SetFloat("rotationX", rot.x);
        animator.SetFloat("rotationY", rot.y);
        animator.SetFloat("dirX", direction.x);
        animator.SetFloat("dirY", direction.y);
    }

    public override void DealDamageToPlayer()
    {
        if (animationDamageDealt)
            return;
        animationDamageDealt = true;

        if (inAttackRange)
            player.DealDamage(this, damage);

        isAttacking = false;
    }
}
