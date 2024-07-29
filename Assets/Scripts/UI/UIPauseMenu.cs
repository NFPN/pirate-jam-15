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

    public Button buttonBack;

    public Slider masterVol;
    public Slider musicVol;
    public Slider sfxVol;

    InventoryControl inventory;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.gameObject.SetActive(false);
        inventory = InventoryControl.inst;
        DataControl.inst.OnLoaded += OnSceneLoaded;

        InputControl.inst.Subscribe("ExitWindow", OnCloseWindowKey);
        InputControl.inst.Subscribe("ExitWindow", OnCloseWindowKey);

        buttonBack.onClick.AddListener(OnButtonBackClick);

        masterVol.onValueChanged.AddListener(OnMasterVolChanged);
        musicVol.onValueChanged.AddListener(OnMusicVolChanged);
        sfxVol.onValueChanged.AddListener(OnSFXValueChanged);


        masterVol.value = masterVol.maxValue;
        musicVol.value = musicVol.maxValue;
        sfxVol.value = sfxVol.maxValue;

    }

    private void OnSFXValueChanged(float value)
    {
        AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.VolSFX, GetNewVolume(value, sfxVol.maxValue));
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIClickSmall);
    }

    private void OnMusicVolChanged(float value)
    {
        AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.VolMX, GetNewVolume(value, musicVol.maxValue));
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIClickSmall);

    }

    private void OnMasterVolChanged(float value)
    {
        AudioControl.inst.SetGlobalParameter(Utils.AudioParameters.VolMaster, GetNewVolume(value,masterVol.maxValue));
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIClickSmall);

    }

    private float GetNewVolume(float sliderValue, float maxValue) => sliderValue / maxValue * 100;

    private void OnButtonBackClick()
    {
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIClickSmall);
        ClosePause();
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
