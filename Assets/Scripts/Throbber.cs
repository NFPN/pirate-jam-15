using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Throbber : MonoBehaviour
{
    public float spinRadius = 1;
    public float spinSpeed = 1;

    public bool isInfinity = false;

    private TrailRenderer trail;
    private Color currentColor;
    private Color currentColorBegin;
    private Color nextColorBegin;
    private Color nextColor;

    public float colorChangeSpeed = 3;
    public float minColorDistance = 0.1f;

    private bool firstUpdate = true;

    private bool inverseX = false;
    private bool hasInversed = false;


    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        currentColor = GenNewColor();
        nextColor = GenNewColor();
        currentColorBegin = GenNewColor();
        nextColorBegin = GenNewColor();
    }

    // Update is called once per frame
    void Update()
    {
        if(firstUpdate)
            trail.enabled = false;


        Vector2 spinPos = Vector2.zero;

        if (isInfinity)
        {
            spinPos.x = Mathf.Cos(Time.time * spinSpeed) * spinRadius;
            spinPos.y = Mathf.Sin(Time.time * spinSpeed) * spinRadius;

            var interval = Time.time * spinSpeed / (4 * Mathf.PI);

            var currentInterval = interval - (int)interval;

            if (inverseX)
            {
                spinPos.x = spinRadius - spinPos.x;
                if(currentInterval < 0.5f)
                    inverseX = false;

            }
            else
            {
                spinPos.x = -spinRadius + spinPos.x;

                if(currentInterval > 0.5f)
                    inverseX = true;
            }
        }
        else
        {
            spinPos = new Vector2(Mathf.Cos(Time.time * spinSpeed) * spinRadius, Mathf.Sin(Time.time * spinSpeed) * spinRadius);
        }

        transform.localPosition = new(spinPos.x, spinPos.y, transform.localPosition.z);

        trail.endColor = currentColor;
        trail.startColor = currentColorBegin;

        LerpToColor();

        if (firstUpdate)
        {
            firstUpdate = false;
            trail.enabled = true;
        }
    }

    private void LerpToColor()
    {
        currentColor = Color.Lerp(currentColor, nextColor, colorChangeSpeed * Time.deltaTime);
        currentColorBegin = Color.Lerp(currentColorBegin, nextColorBegin, colorChangeSpeed * Time.deltaTime);
        if (Vector3.Distance(new(currentColorBegin.r, currentColorBegin.g, currentColorBegin.b), new(nextColorBegin.r, nextColorBegin.g, nextColorBegin.b)) < minColorDistance)
            nextColorBegin = GenNewColor();

        if (Vector3.Distance(new(currentColor.r, currentColor.g, currentColor.b), new(nextColor.r, nextColor.g, nextColor.b)) < minColorDistance)
            nextColor = GenNewColor();
    }

    private Color GenNewColor()
    {
        return  new(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
    }
}
