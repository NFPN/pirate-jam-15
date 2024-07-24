using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using static UnityEditor.Rendering.InspectorCurveEditor;

[RequireComponent(typeof(Animator))]
public class ShardVisualControl : MonoBehaviour
{
    public Image shardImage;
    public TextMeshProUGUI shardText;

    public Sprite shadowShard;
    public Sprite normalShard;

    private Animator animator;
    private Material transitionMat;

    private float fillAmount = 1.0f;

    private int shardCount;

    public float animationUpdateInterval = 0.01f;
    public float animationSpeed = 1.5f;

    public float textDitherIntensity = 1.2f;

    private Coroutine shardAnimCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        shardCount = InventoryControl.inst.ShardCount;
        UpdateShardVisual();
        InventoryControl.inst.OnShardsAmountChanged += UpdateShadardCount;

        WorldShaderControl.inst.OnWorldChangeBegin += OnWorldChangeBegin;


        transitionMat = Instantiate(shardImage.material);
        shardImage.material = transitionMat;

        VisualStateChange(WorldShaderControl.inst.IsShadowWorld);
    }

    private void OnWorldChangeBegin(bool isShadow)
    {
        if(shardAnimCoroutine != null) 
            StopCoroutine(shardAnimCoroutine);
        shardAnimCoroutine = StartCoroutine(ShardAnimation(isShadow));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateShadardCount(int count)
    {
        shardCount = count;
        animator.Play("ShardAnimation");
    }
    public void UpdateShardVisual()
    {
        shardText.text = shardCount.ToString();
    }

    IEnumerator ShardAnimation(bool isShadow)
    {
        while (fillAmount > 0)
        {
            yield return new WaitForSeconds(animationUpdateInterval);
            fillAmount -= GetAnimationIncrement();
            transitionMat.SetFloat("_VisibleAmount", fillAmount);
            shardText.alpha = GetTextTransparency();
        }
        VisualStateChange(isShadow);
        while (fillAmount < 1)
        {
            yield return new WaitForSeconds(animationUpdateInterval);
            fillAmount += GetAnimationIncrement();
            fillAmount = Mathf.Clamp01(fillAmount);
            transitionMat.SetFloat("_VisibleAmount", fillAmount);
            shardText.alpha = GetTextTransparency();

        }
        shardText.alpha = 1;
    }

    float GetAnimationIncrement()
    {
        if (fillAmount < 0.5f)
            return animationUpdateInterval * animationSpeed * .5f;
        return animationUpdateInterval * animationSpeed * 2;
    }

    private void VisualStateChange(bool isShadow)
    {
        if (isShadow)
        {
            shardImage.sprite = shadowShard;
        }
        else
        {
            shardImage.sprite = normalShard;
        }
    }

    private float GetTextTransparency()
    {
        var result = Mathf.Pow(fillAmount, 3);
        result += UnityEngine.Random.Range(-result * textDitherIntensity, result * textDitherIntensity);
        return result;
    }
}
