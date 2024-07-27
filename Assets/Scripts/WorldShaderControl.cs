using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldShaderControl : MonoBehaviour
{
    public static WorldShaderControl inst;
    // Controls the Parameters of shaders that are applied many times on different objects (ex: world tiles)
    // Controls the transition between worlds

    public event Action<bool> OnWorldChangeBegin;
    public event Action<bool> OnChangeSpriteVisual;
    public event Action<bool> OnDeathWorldChange;
    public event Action OnSceneLeave;
    public event Action OnWorldChangeComplete;

    public event Action OnUpdateIsPlayerControllable;

    [Header("Transition Effect")]
    public Material playerMaterial;
    public List<Material> transitionMaterials;
    public float effectSpeed = 0.2f;
    public float updateInterval = 0.01f;
    public float deathMulti = 0.3f;

    private bool isShadowWorld = false;
    private bool isChangingState = false;
    private float spriteFill = 1.0f;

    private bool isDeathReload = false;

    public bool IsShadowWorld { get { return isShadowWorld; } }
    public bool IsDeathReload { get => isDeathReload; }

    private bool isPlayerControllable;


    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);


        var persistentData = DataControl.inst;
        if (persistentData)
            isShadowWorld = persistentData.IsShadowWorld;
    }

    // Start is called before the first frame update
    void Start()
    {
        InputControl.inst.Subscribe("Transcend", ChangeWorlds);

        var deathReload = DataControl.inst.IsReloadOnDeath();
        if (deathReload)
        {
            isDeathReload = true;
            //OnWorldChangeBegin?.Invoke(isShadowWorld);
            //OnDeathWorldChange?.Invoke(isShadowWorld);
            StartCoroutine(DeathChangeWorldAnimation());
        }
        else
            SetupWorld();
    }


    private void OnDisable()
    {
        InputControl.inst.Unsubscribe("Transcend", ChangeWorlds);
    }

    private void SetupWorld()
    {
        spriteFill = 1.0f;
        if (playerMaterial)
        {
            playerMaterial.SetInt("_ToCenter", 1);
            playerMaterial.SetInt("_ToY", 0);
        }
        UpdateShaderParams();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SceneLeave()
    {
        isChangingState = true;
        OnSceneLeave();
    }

    public void ChangeWorlds(InputAction.CallbackContext callbackContext)
    {
        OnUpdateIsPlayerControllable?.Invoke();

        if (!isPlayerControllable)
            return;
        if (callbackContext.phase != InputActionPhase.Started || isChangingState)
            return;

        var changeWorldData = InventoryControl.inst.GetAbilityData(Utils.Abilities.Transcend);

        if (changeWorldData.IsLocked)
            return;

        isShadowWorld = !isShadowWorld;

        AudioControl.inst.PlayOneShot(Utils.SoundType.WorldChange);

        OnWorldChangeBegin?.Invoke(isShadowWorld);
        StartCoroutine(ChangeWorldAnimation());
    }

    public void UpdatePlayerControllable(bool isControllable)
    {
        isPlayerControllable = isControllable;
    }

    private void UpdateShaderParams()
    {
        float minFill;
        float maxFill;
        if (playerMaterial)
        {
            maxFill = playerMaterial.GetFloat("_MaxFill");
            minFill = playerMaterial.GetFloat("_MinFill");
            // total fill anount offset by bottom bound  (max -min)*fill +min
            playerMaterial.SetFloat("_FillAmount", (maxFill - minFill) * spriteFill + minFill);
        }
        foreach (Material mat in transitionMaterials)
        {
            maxFill = mat.GetFloat("_MaxFill");
            minFill = mat.GetFloat("_MinFill");
            // total fill anount offset by bottom bound  (max -min)*fill +min
            mat.SetFloat("_FillAmount", (maxFill - minFill) * spriteFill + minFill);
        }
    }

    IEnumerator ChangeWorldAnimation()
    {
        isChangingState = true;
        // Disappear 
        while (spriteFill > 0)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            spriteFill -= effectSpeed * updateInterval;
            UpdateShaderParams();
        }
        spriteFill = 0;
        UpdateShaderParams();

        OnChangeSpriteVisual?.Invoke(isShadowWorld);
        // Emerge
        while (spriteFill < 1.0f)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            spriteFill += effectSpeed * updateInterval;
            UpdateShaderParams();
        }
        spriteFill = 1;
        UpdateShaderParams();
        OnWorldChangeComplete?.Invoke();
        isChangingState = false;
    }

    IEnumerator DeathChangeWorldAnimation()
    {
        if (playerMaterial)
        {
            playerMaterial.SetInt("_ToCenter", 0);
            playerMaterial.SetInt("_ToY", 1);
        }
        isChangingState = true;
        // Disappear 
        spriteFill = 0;
        UpdateShaderParams();

        OnChangeSpriteVisual?.Invoke(isShadowWorld);
        // Emerge
        while (spriteFill < 1.0f)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            spriteFill += effectSpeed * updateInterval * deathMulti;
            UpdateShaderParams();
        }
        OnWorldChangeComplete?.Invoke();
        isChangingState = false;

        if (playerMaterial)
        {
            playerMaterial.SetInt("_ToCenter", 1);
            playerMaterial.SetInt("_ToY", 0);
        }
    }

}
