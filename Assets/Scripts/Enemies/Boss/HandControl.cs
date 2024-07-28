using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour
{
    private Animator animator;

    public bool IsAttacking { get; private set; }
    public bool CanAttack { get; set; }
    public Vector2 TargetPostion { get; set; }
    public Vector2 CurrentPosition { get; set; }

    private float attackSpeed;
    private float normalSpeed;
    private float cooldown;
    private float lastAttack;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (lastAttack + cooldown < Time.time && !IsAttacking)
            CanAttack = true;

        if(IsAttacking)
            CurrentPosition = Vector2.Lerp(CurrentPosition, TargetPostion, attackSpeed * Time.deltaTime);
        else
            CurrentPosition = Vector2.Lerp(CurrentPosition, TargetPostion, normalSpeed * Time.deltaTime);
    }

    public void SetAttackSpeed(float speed)
    {
        attackSpeed = speed;
    }
    public void SetNormalSpeed(float speed)
    {
        normalSpeed = speed;
    }
    public void SetCooldown(float cooldown)
    {
        this.cooldown = cooldown;
    }

    public void AttackEnd()
    {
        IsAttacking = false;
        CanAttack = false;
        lastAttack = Time.time;
    }

    public void Attack(Player target)
    {
        IsAttacking = true;
        CanAttack = false;
        TargetPostion = target.transform.position;
        animator.Play("Attack");
    }
}
