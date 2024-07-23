using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataControl : MonoBehaviour
{
    public static DataControl inst;
    private float playerHealth;

    public float playerHealthMax = 6;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO: connect player to data control (so that player health does not reset every scene and is managed from here)
    public void UpdatePlayerHealth(float playerHealth)
    {
        this.playerHealth = playerHealth;
    }

    public void AddPlayerMaxHealth(float healthAmount)
    {
        playerHealthMax += healthAmount;
    }
    
}
