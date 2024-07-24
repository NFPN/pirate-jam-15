using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeartVisualControl : MonoBehaviour
{
    public Image heartImage;
    private Dictionary<Utils.HeartState, Sprite> sprites;
    private Utils.HeartState curState = Utils.HeartState.None;
    private Utils.HeartState newState = Utils.HeartState.None;

    [Header("Heart Animation")]
    public float animationUpdateInterval;
    public float animationSpeed;
    private float fillAmount = 0;

    private Material transitionMat;

    private bool isAnimation = false;
    // Start is called before the first frame update
    void Start()
    {
        transitionMat = Instantiate(heartImage.material);
        heartImage.material = transitionMat;
    }

    public void SetSprites(Dictionary<Utils.HeartState, Sprite> sprites) => this.sprites = sprites;

    public void ChangeHeart(Utils.HeartState state, bool doAnimation = true)
    {
        newState = state;

        if (!doAnimation)
        {
            SetHeartSprite();
        }
        else if (newState != curState && !isAnimation && doAnimation)
            StartCoroutine(HeartAnimation());

    }

    IEnumerator HeartAnimation()
    {
        isAnimation = true;
        if(curState == Utils.HeartState.None)
            fillAmount = 0;

        if(curState == Utils.HeartState.Full && newState == Utils.HeartState.Half || curState == Utils.HeartState.Half && newState == Utils.HeartState.Full)
        {
            SetHeartSprite();
            isAnimation = false;
            yield break;
        }
        if (curState == Utils.HeartState.FullShadow && newState == Utils.HeartState.HalfShadow || curState == Utils.HeartState.HalfShadow && newState == Utils.HeartState.FullShadow)
        {
            SetHeartSprite();
            isAnimation = false;
            yield break;
        }

        while (fillAmount > 0)
        {
            yield return new WaitForSeconds(animationUpdateInterval);
            fillAmount -= GetAnimationIncrement();
            transitionMat.SetFloat("_VisibleAmount", fillAmount);
        }

        while (fillAmount < 1)
        {
            yield return new WaitForSeconds(animationUpdateInterval);
            fillAmount += GetAnimationIncrement();
            fillAmount = Mathf.Clamp01(fillAmount);
            transitionMat.SetFloat("_VisibleAmount", fillAmount);

            SetHeartSprite();
        }
        isAnimation = false;
    }

    private void SetHeartSprite()
    {
        if (curState == newState)
            return;
        fillAmount = 1;
        curState = newState;
        heartImage.sprite = sprites[curState];

        if (curState == Utils.HeartState.None)
            heartImage.enabled = false;
        else
            heartImage.enabled = true;
    }


    float GetAnimationIncrement()
    {
        if (fillAmount < 0.5f)
            return animationUpdateInterval * animationSpeed * .5f;
        return animationUpdateInterval * animationSpeed * 2;
    }
}
