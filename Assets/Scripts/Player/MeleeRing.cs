using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRing : DamageObject
{
    public List<MeleeRingLevel> levels;

    public float duration = 0.2f;
    public float startRadius = 0.5f;


    private MeleeRingLevel currentLevel;

    private CircleCollider2D circleCollider;

    private bool isAttacking;
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = levels[0];
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;
    }

    public void AOEAttack(int level)
    {
        if (isAttacking)
            return;

        currentLevel = levels[level - 1];

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        circleCollider.enabled = true;

        //TODO: Add settings to class
        var targetRadius = currentLevel.range;
        var elapsedTime = 0f;
        damage = currentLevel.damage;

        circleCollider.radius = startRadius;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            circleCollider.radius = Mathf.Lerp(startRadius, targetRadius, elapsedTime / duration);
            yield return null;
        }

        circleCollider.radius = targetRadius;
        circleCollider.enabled = false;

        yield return new WaitForSeconds(currentLevel.attackDelay);

        isAttacking = false;
    }
}

[System.Serializable]
public struct MeleeRingLevel
{
    public int damage;
    public float range;
    public float attackDelay;
}
