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

        InputControl.inst.Subscribe("ExitWindow", OnCloseWindowKey);
        InputControl.inst.Subscribe("ExitWindow", OnCloseWindowKey);
    }

    private void OnSceneLoaded()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void OnEnable()
    {
        if(InputControl.inst != null)
            InputControl.inst.Subscribe("ExitWindow", OnCloseWindowKey);
    }

    private void OnDisable()
    {
        InputControl.inst.Unsubscribe("ExitWindow", OnCloseWindowKey);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnCloseWindowKey(InputAction.CallbackContext callbackContext)
    {
        if (inventory.WindowCloseTime == Time.time)
            return;
        if (inventory.WindowOpen && !pauseOpen)
            return;

        if (callbackContext.phase == InputActionPhase.Started)
        {
            if (pauseOpen)
                ClosePause();
            else
                OpenPause();
        }
        
    }

    private void ClosePause()
    {
        Time.timeScale = 1;
        pauseOpen = false;
        pauseMenu.gameObject.SetActive(false);
        inventory.WindowOpen = false;

        if (player)
            player.DisablePlayerControls(false);
    }
    private void OpenPause()
    {
        Time.timeScale = 0;
        pauseOpen = true;
        pauseMenu.gameObject.SetActive(true);
        inventory.WindowOpen = true;
        if (player)
        {
            print("controls disabled");
            player.DisablePlayerControls(true);
        }
    }
}
