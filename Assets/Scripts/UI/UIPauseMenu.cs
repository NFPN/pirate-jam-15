using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    private bool pauseOpen = false;

    private Player player;

    public Image pauseMenu;
    InventoryControl inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = InventoryControl.inst;
        DataControl.inst.OnLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded()
    {
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnCloseWindowKey(InputAction.CallbackContext callbackContext)
    {
        /*
        if (callbackContext.phase == InputActionPhase.Started && !inventory.WindowOpen)
        {
            if (pauseOpen)
            {
                Time.timeScale = 1;
                pauseOpen = false;
                pauseMenu.gameObject.SetActive(false);
                inventory.WindowOpen = false;

                if (player)
                    player.DisablePlayerControls(false);
            }
            else
            {
                Time.timeScale = 0;
                pauseOpen = true;
                pauseMenu.gameObject.SetActive(true);
                inventory.WindowOpen = true;
                if (player)
                    player.DisablePlayerControls(true);
            }
        }
        */
    }

}
