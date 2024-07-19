using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeartVisualControl : MonoBehaviour
{
    public Image heartImage;
    private Dictionary<Utils.HeartState, Sprite> sprites;
    private Utils.HeartState curState = Utils.HeartState.None;

    private bool isShadow = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetSprites(Dictionary<Utils.HeartState, Sprite> sprites) => this.sprites = sprites;

    public void ChangeHeart(Utils.HeartState state)
    {
        curState = state;
        heartImage.sprite = sprites[curState];

        if (curState == Utils.HeartState.None)
            heartImage.enabled = false;
        else
            heartImage.enabled = true;

    }

}
