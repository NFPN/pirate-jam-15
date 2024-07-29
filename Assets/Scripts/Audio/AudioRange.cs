using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRange : MonoBehaviour
{
    private bool isEnemyInRange = false;

    private int enemyCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(!isEnemyInRange)
            {
                isEnemyInRange = true;
                AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.AreEnemiesAround, 1);
            }
            enemyCount++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(isEnemyInRange && enemyCount == 1)
            {
                isEnemyInRange = false;
                AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.AreEnemiesAround, 0);
            }
            enemyCount--;
        }
    }

}
