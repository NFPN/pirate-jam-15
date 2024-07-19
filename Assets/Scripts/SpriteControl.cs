using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteControl : MonoBehaviour
{
    private SpriteRenderer render;
    public Material transitionMaterial;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.RegisterSpriteChangeCallback(OnSpriteChanged);
        OnSpriteChanged(render);
    }

    private void OnSpriteChanged(SpriteRenderer arg0)
    {
        var textureX = render.sprite.texture.width;
        var textureY = render.sprite.texture.height;

        var spriteOffset = render.sprite.rect.position;
        var spriteSize = render.sprite.rect.size;

        var spriteOffsetUv = new Vector2(spriteOffset.x / textureX, spriteOffset.y / textureY);
        var spriteSizeUv = new Vector2(spriteSize.x / textureX, spriteSize.y / textureY);

        transitionMaterial.SetVector("_SpriteOffset", spriteOffsetUv);
        transitionMaterial.SetVector("_SpriteSize", spriteSizeUv);
    }
}
