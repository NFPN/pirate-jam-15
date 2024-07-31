using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyIndicatorControl : MonoBehaviour
{
    public static KeyIndicatorControl inst;

    public TextMeshProUGUI textHolder;

    public Transform target;
    private GameObject callerObject;
    private Vector2 callerOffest;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);

        textHolder.enabled = false;

    }

    public void Update()
    {
        UpdateKeyIndicatorPosition();
    }

    private void UpdateKeyIndicatorPosition()
    {
        if (!callerObject)
            return;

        Vector3 newPos = callerObject.transform.position;

        newPos += new Vector3(callerOffest.x, - textHolder.rectTransform.sizeDelta.y/2, 0);

        transform.position = newPos;

    }

    public void ShowIndicator(Utils.Iteraction interaciton, GameObject source, Vector2 indicatorOffset)
    {
        textHolder.enabled = true;
        string key = "";
        switch (interaciton)
        {
            case Utils.Iteraction.Interact:
                key = "Z";
                break;
            case Utils.Iteraction.Attack:
                break;
            case Utils.Iteraction.Dash:
                break;
            case Utils.Iteraction.SwitchWorld:
                break;
            default:
                break;
        }

        textHolder.text = key;
        callerObject = source;
        callerOffest = indicatorOffset;

        UpdateKeyIndicatorPosition();
    }
    public void HideIndicator()
    {
        if (textHolder == null)
            return;
        textHolder.enabled = false;
    }
}
