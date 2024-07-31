using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControl : MonoBehaviour
{
    private Animator animator;
    private Collider2D collide;

    public bool IsAttacking { get; private set; }
    public bool CanAttack { get; set; }
    public Vector2 TargetPostion { get; set; }
    public Vector2 CurrentPosition { get; set; }
    public float AttackDelay { get; set; }
    public float ColliderDelay { get; set; }
    public int DamageDealt { get; set; }

    public BossStats Stats { get; set; }

    private float attackSpeed;
    private float normalSpeed;
    private float cooldown;
    private float lastAttack;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collide = GetComponent<Collider2D>();
        collide.enabled = false;
        AttackDelay = 0.7f;
        ColliderDelay = 0.2f;
        DamageDealt = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastAttack + Stats.cooldown < Time.time && !IsAttacking)
            CanAttack = true;

        if(IsAttacking)
            CurrentPosition = Vector2.Lerp(CurrentPosition, TargetPostion, Stats.handMoveSpeedAttack * Time.deltaTime);
        else
            CurrentPosition = Vector2.Lerp(CurrentPosition, TargetPostion, Stats.handMoveSpeed * Time.deltaTime);

        animator.speed = Stats.handMoveSpeedAttack/2;
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

    public void DealDamage()
    {
        StartCoroutine(DamageAreaDelay());
    }
    public void PlayAttackSound()
    {
        AudioControl.inst.PlayOneShot(Utils.SoundType.BossAttack, transform.position);
    }

    public void AttackEnd()
    {
        StartCoroutine(AttackEndDelay());   
    }
    private IEnumerator AttackEndDelay()
    {
        yield return new WaitForSeconds(Stats.attackDelay);
        IsAttacking = false;
        CanAttack = false;
        lastAttack = Time.time;

    }

    private IEnumerator DamageAreaDelay()
    {
        collide.enabled = true;
        yield return new WaitForSeconds(ColliderDelay);
        collide.enabled = false;
    }

    public void Attack(Player target)
    {
        IsAttacking = true;
        CanAttack = false;
        TargetPostion = target.transform.position;
        player = target;
        animator.Play("Attack");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.DealDamage(this, DamageDealt);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.DealDamage(this, DamageDealt);
        }
            
    }
}
