using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drop : MonoBehaviour
{
    public Utils.Items dropItem;
    public float collectSpeed;
    public float rotationSpeed;
    public float rotChangeSpeed;

    public bool doRotation = true;
    public bool scaleOnStart = true;

    private DropCollector player;
    private float minDistance;

    private Vector3 nextRot;

    private float speedOffset;
    private float lastRotChange;

    public float scaleSpeed = 1;
    private Vector3 defaultScale;

    private void Start()
    {
        if (scaleOnStart)
        {
            defaultScale = transform.localScale;
            transform.localScale = new(0, 0, 0);
            StartCoroutine(SpawnDrop());
        }
    }

    public void CollectDrop(DropCollector player, float minDistance)
    {
        GetComponent<Collider2D>().enabled = false;

        this.player = player;
        this.minDistance = minDistance;

        StartCoroutine(GravitateToPlayer());
    }

    IEnumerator SpawnDrop()
    {
        while(Vector3.Distance(transform.localScale, defaultScale) > 0.009f)
        {
            yield return new WaitForSeconds(0.01f);
            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, scaleSpeed * 0.01f);
        }

        transform.localScale = defaultScale;
    }

    IEnumerator GravitateToPlayer()
    {
        speedOffset = Random.Range(-collectSpeed * 0.1f, collectSpeed * 0.1f);

        while (Vector3.Distance(player.transform.position, transform.position) > minDistance)
        {
            yield return new WaitForSeconds(0.01f);

            var dir = (player.transform.position - transform.position).normalized;
            transform.position += dir * (collectSpeed+speedOffset) * 0.01f;

            if (doRotation)
            {

                if (lastRotChange + rotChangeSpeed < Time.time)
                {
                    lastRotChange = Time.time;
                    nextRot = GetRandomRotation();
                }

                var rotDir = (nextRot - transform.rotation.eulerAngles).normalized;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotDir * rotationSpeed * 0.01f);
            }
        }

        player.ProcessDrop(this);
    }


    public void DestroyDrop()
    {
        Destroy(gameObject);
    }

    private Vector3 GetRandomRotation()
    {
        return new Vector3(Random.Range(0, 40.0f), Random.Range(0, 40.0f), Random.Range(0, 360.0f));
    }
}
