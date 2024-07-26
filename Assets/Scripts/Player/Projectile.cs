using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : DamageObject
{
    public float velocity = 10;

    public float disapperRate = 1;
    private Rigidbody2D rb2D;
    private Vector2 direction;

    private float castTime;
    private float duration;

    private Animator animator;

    private bool isMoving;
    private bool isUpdating;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collision = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!isUpdating)
            return;

        if (rb2D == null || direction == Vector2.zero)
        {
            gameObject.SetActive(false);
            return;
        }

        if (duration + castTime < Time.time)
        {
            isUpdating = false;
            StartCoroutine(DisappearEffect());
            return;
        }

        if (isMoving)
            rb2D.velocity = velocity * direction;
    }

    public void SetProjectileStats(int damage, float speed, float duration, Vector2 size)
    {
        this.damage = damage;
        velocity = speed;
        this.duration = duration;
        transform.localScale = size.ToVector3();

        castTime = Time.time;
        isMoving = true;
        isUpdating = true;

        collision.enabled = true;
    }

    public void SetDirection(Vector2 direction) => this.direction = direction;

    protected override void PlayOnCollisionAnimation()
    {
        isMoving = false;
        rb2D.velocity = Vector2.zero;
        collision.enabled = false;
        animator.Play("Collision");
    }

    private IEnumerator DisappearEffect()
    { 
        collision.enabled = false;
        while (transform.localScale.magnitude > 0.1f)
        {
            yield return new WaitForSeconds(0.01f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, disapperRate * 0.01f);
        }
        gameObject.SetActive(false);
    }
}
