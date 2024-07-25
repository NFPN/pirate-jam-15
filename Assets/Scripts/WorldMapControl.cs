using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapControl : MonoBehaviour
{
    public static WorldMapControl inst;

    public Material groundTransformMaterial;
    public Material backplateMat;
    public Material sceneChangeMat;

    public GameObject normalWorld;
    public GameObject shadowWorld;
    public GameObject backplate;
    public GameObject sceneChange;

    private float spriteFill = 1;
    public float updateInterval = 0.01f;
    public float effectSpeed = 1;
    public float initialDelay = 0.1f;
    public float deathMulti = 0.3f;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        backplate.SetActive(false);
        WorldShaderControl.inst.OnWorldChangeBegin += ChangeWorldMap;
        WorldShaderControl.inst.OnWorldChangeComplete += ChangeWorldComplete;
        WorldShaderControl.inst.OnSceneLeave += OnSceneLeave;

        groundTransformMaterial.SetInt("_IsShadow", 0);

        var deathReload = WorldShaderControl.inst.IsDeathReload;
        if (deathReload)
            StartCoroutine(DeathChangeWorldAnimation(WorldShaderControl.inst.IsShadowWorld));
        else
            SetupWorld(WorldShaderControl.inst.IsShadowWorld);
    }

    private void OnSceneLeave()
    {
        StartCoroutine(SceneLeaveAnimation());
    }

    private void ChangeWorldComplete()
    {

    }

    private void ChangeWorldMap(bool isShadow)
    {
        groundTransformMaterial.SetInt("_IsShadow", isShadow ? 1 : 0);
        StartCoroutine(ChangeWorldAnimation(isShadow));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ChangeWorldAnimation(bool isShadow)
    {

        yield return new WaitForSeconds(initialDelay);
        // Disappear 
        while (spriteFill > 0)
        {
            yield return new WaitForSeconds(updateInterval);
            spriteFill -= effectSpeed * updateInterval;
            UpdateShaderParams();
        }

        shadowWorld.SetActive(isShadow);
        normalWorld.SetActive(!isShadow);

        // Emerge
        while (spriteFill < 1.0f)
        {
            yield return new WaitForSeconds(updateInterval);
            spriteFill += effectSpeed * updateInterval;
            UpdateShaderParams();
        }
    }

    IEnumerator DeathChangeWorldAnimation(bool isShadow)
    {
        backplate.SetActive(true);
        sceneChange.SetActive(false);

        spriteFill = 0;
        UpdateShaderParams();


        shadowWorld.SetActive(isShadow);
        normalWorld.SetActive(!isShadow);

        // Emerge
        while (spriteFill < 1.0f)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            spriteFill += effectSpeed * updateInterval * deathMulti;
            backplateMat.SetFloat("_FillAmount", spriteFill);
            UpdateShaderParams();
        }

        backplate.SetActive(false);
    }

    IEnumerator SceneEnterAnimaion()
    {
        sceneChange.SetActive(true);
        spriteFill = 0;
        UpdateShaderParams();

        // Emerge
        while (spriteFill < 1.0f)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            spriteFill += effectSpeed * updateInterval;
            sceneChangeMat.SetFloat("_FillAmount", spriteFill);
            UpdateShaderParams();
        }
        sceneChange.SetActive(false);
    }

    IEnumerator SceneLeaveAnimation()
    {
        sceneChange.SetActive(true);
        spriteFill = 1;
        UpdateShaderParams();

        // Emerge
        while (spriteFill > 0)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            spriteFill -= effectSpeed * updateInterval;
            sceneChangeMat.SetFloat("_FillAmount", spriteFill);
            UpdateShaderParams();
        }

    }

    private void UpdateShaderParams()
    {
        spriteFill = Mathf.Clamp01(spriteFill);
        groundTransformMaterial.SetFloat("_FillAmount", spriteFill);
        backplateMat.SetFloat("_FillAmount", spriteFill);
        sceneChangeMat.SetFloat("_FillAmount", spriteFill);
    }


    private void SetupWorld(bool isShadow)
    {
        shadowWorld.SetActive(isShadow);
        normalWorld.SetActive(!isShadow);
        UpdateShaderParams();
        StartCoroutine(SceneEnterAnimaion());
    }
}
