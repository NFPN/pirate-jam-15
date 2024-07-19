using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileMapControl : MonoBehaviour
{
    private TilemapRenderer render;
    private Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        SetupTransitionMaterial();
    }

    private void SetupTransitionMaterial()
    {

    }
}
