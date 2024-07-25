using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : Enemy
{
    private bool playerInRange;
    private bool inAttackRange;

    public float attackSpeed;
    private float lastAttackTime;

    public SpriteRenderer enemyVisual;

    private bool inMinDist;

    private void Start()
    {
        OnDeath += EnemyDeath;
        playerDetection.OnPlayerInRange += () => { playerInRange = true; DoMovement(); };
        playerDetection.OnPlayerOutOfRange += () => { playerInRange = false; StopMovement(); };

        attackRange.OnPlayerInRange += () => inAttackRange = true;
        attackRange.OnPlayerOutOfRange += () => inAttackRange = false;

        HasAttackAnim = true;
        HasKnockbackAnim = true;
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
        if(Vector3.Distance(transform.position, player.transform.position)> minDistanceToPlayer && inMinDist)
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
        var rot = (player.transform.position - transform.position).normalized;
        animator.SetFloat("rotationX", rot.x);
        animator.SetFloat("rotationY", rot.y);
    }
}
