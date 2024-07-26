using System.Linq;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public string[] AvoidTags;
    public float damage = 1;
    public bool deactivateOnCollision;

    protected Collider2D collision;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!AvoidTags.Any(x => collision.gameObject.CompareTag(x)))
        {
            var health = collision.gameObject.GetComponent<IHealth>();
            health?.DealDamage(this, damage);
            if (deactivateOnCollision)
                PlayOnCollisionAnimation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!AvoidTags.Any(x => collision.gameObject.CompareTag(x)))
        {
            var health = collision.gameObject.GetComponent<IHealth>();
            health?.DealDamage(this, damage);
            if (deactivateOnCollision)
                PlayOnCollisionAnimation();
        }
    }

    protected virtual void PlayOnCollisionAnimation()
    {
        DestroyObject();
    }
    public void DestroyObject()
    {
        gameObject.SetActive(false);
    }
}