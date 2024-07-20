using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float velocity = 10;
    public string[] AvoidTags;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!AvoidTags.Any(x => collision.gameObject.CompareTag(x)))
            gameObject.SetActive(false);
    }

    public void SetDirection(Vector2 direction) => this.direction = direction;
}
