using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : Enemy
{
    private bool playerInRange;
    private bool inAttackRange;

    public float attackSpeed;
    private float lastAttackTime;

    public SpriteRenderer enemyVisual;

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

        if (Vector3.Distance(transform.position, player.transform.position) < minDistanceToPlayer) 
            StopMovement();
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

        AttackAnimation();
    }

    private void UpdateSlimeFacing()
    {
        var rot = transform.rotation.eulerAngles;
        if (player.transform.position.x < transform.position.x)
        {
            rot.y = 0;
        }
        else
        {
            rot.y = 180;
        }
        transform.rotation = Quaternion.Euler(rot);
    }
}
