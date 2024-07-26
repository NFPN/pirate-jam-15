using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputControl : MonoBehaviour
{
    public static InputControl inst;

    public PlayerInput input { get; private set; }

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
            input = GetComponent<PlayerInput>();
        }
        else
            Destroy(gameObject);
    }

    public void Subscribe(string name, Action<InputAction.CallbackContext> callback)
    {
        if (!input)
            return;
        input.actions[name].started += callback;
        input.actions[name].performed += callback;
        input.actions[name].canceled += callback;
    }
    public void Unsubscribe(string name, Action<InputAction.CallbackContext> callback)
    {
        if (!input)
            return;
        input.actions[name].started -= callback;
        input.actions[name].performed -= callback;
        input.actions[name].canceled -= callback;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
