using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : DamageObject
{
    public float velocity = 10;
    private Rigidbody2D rb2D;
    private Vector2 direction;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (rb2D == null || direction == Vector2.zero)
            return;

        rb2D.velocity = velocity * direction;
    }

    public void SetDirection(Vector2 direction) => this.direction = direction;
}
