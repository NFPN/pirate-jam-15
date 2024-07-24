using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlimeEnemy : Enemy
{

    private void Start()
    {
        OnDeath += Died;

        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            navAgent.SetDestination(player.transform.position);
        }
    }
}
