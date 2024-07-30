using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHover : MonoBehaviour
{
    public float xIntensity = 0.1f;
    public float yIntensity = 0.1f;

    public float xSpeed = 0.6f;
    public float ySpeed = 1.4f;

    private Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = startPos + new Vector2(Mathf.Sin(Time.time * xSpeed) * xIntensity, Mathf.Sin(Time.time * ySpeed) * yIntensity);
    }
}
