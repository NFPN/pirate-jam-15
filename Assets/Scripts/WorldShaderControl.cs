using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class WorldShaderControl : MonoBehaviour
{
    public static WorldShaderControl inst;
    // Controls the Parameters of shaders that are applied many times on different objects (ex: world tiles)
    // Controls the transition between worlds

    public event Action<bool> OnWorldChangeBegin;
    public event Action<bool> OnChangeSpriteVisual;
    [Header("Transition Effect")]
    public Action OnWorldChangeComplete;
    public List<Material> transitionMaterials;
    public float effectSpeed = 0.2f;
    public float updateInterval = 0.01f;
    [HideInInspector] public bool isShadowWorld = false;
    private bool isChangingState = false;
    private float spriteFill = 1.0f;


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

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ChangeWorlds();
    }

    public void ChangeWorlds()
    {
        if (isChangingState)
            return;
        isShadowWorld = !isShadowWorld;

        OnWorldChangeBegin?.Invoke(isShadowWorld);
        StartCoroutine(ChangeWorldAnimation());
    }

    private void UpdateShaderParams()
    {
        foreach (Material mat in transitionMaterials)
        {
            var maxFill = mat.GetFloat("_MaxFill");
            var minFill = mat.GetFloat("_MinFill");
            // total fill anount offset by bottom bound  (max -min)*fill +min
            mat.SetFloat("_FillAmount", (maxFill - minFill)*spriteFill + minFill);
        }
    }

    IEnumerator ChangeWorldAnimation() 
    {
        isChangingState = true;
        // Disappear 
        while (spriteFill > 0)
        {
            yield return new WaitForSeconds(updateInterval);
            spriteFill -= effectSpeed * updateInterval;
            UpdateShaderParams();
        }
        OnChangeSpriteVisual?.Invoke(isShadowWorld);
        // Emerge
        while(spriteFill < 1.0f)
        {
            yield return new WaitForSeconds(updateInterval);
            spriteFill += effectSpeed * updateInterval;
            UpdateShaderParams();
        }
        OnWorldChangeComplete?.Invoke();
        isChangingState = false;
    }

}
