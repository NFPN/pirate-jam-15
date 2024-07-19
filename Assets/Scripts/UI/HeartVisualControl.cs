using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class HeartVisualControl : MonoBehaviour
{
    public Image heartImage;
    public Animator Animator { get; private set; }
    private Dictionary<Utils.HeartState, Sprite> sprites;
    private Utils.HeartState curState = Utils.HeartState.None;

    private bool isShadow = false;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSprites(Dictionary<Utils.HeartState, Sprite> sprites) => this.sprites = sprites;

    public void ChangeHeart(Utils.HeartState state)
    {

        // Via Math we get the index of the correct animation to display based on current state and new state

        int animationIndex = (int)state;

        animationIndex += (int)curState * 3;
        if (isShadow)
            animationIndex *= 2;

        curState = state;
        Animator.SetInteger("heartState", animationIndex);
    }

    public void SetSprite()
    {
        Animator.SetInteger("heartState", 0);
        heartImage.sprite = sprites[curState];
    }
}
