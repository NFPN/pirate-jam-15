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

    public void ChangeHeart(Utils.HeartState state)
    {
        newState = state;

        if (newState != curState && !isAnimation)
            StartCoroutine(HeartAnimation());
    }

    IEnumerator HeartAnimation()
    {
        isAnimation = true;
        while (fillAmount > 0)
        {
            yield return new WaitForSeconds(animationUpdateInterval);
            fillAmount -= GetAnimationIncrement();
            transitionMat.SetFloat("_VisibleAmount", fillAmount);
        }

        curState = newState;
        heartImage.sprite = sprites[curState];

        if (curState == Utils.HeartState.None)
            heartImage.enabled = false;
        else
            heartImage.enabled = true;


        while (fillAmount < 1)
        {
            yield return new WaitForSeconds(animationUpdateInterval);
            fillAmount += GetAnimationIncrement();
            fillAmount = Mathf.Clamp01(fillAmount);
            transitionMat.SetFloat("_VisibleAmount", fillAmount);
        }
        isAnimation = false;
    }


    float GetAnimationIncrement()
    {
        if (fillAmount < 0.5f)
            return animationUpdateInterval * animationSpeed * .5f;
        return animationUpdateInterval * animationSpeed * 2;
    }
}
