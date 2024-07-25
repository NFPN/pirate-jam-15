using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSaveData : MonoBehaviour
{
    [SerializeField]private string id;
    public string ID {  get; private set; }

    private void Awake()
    {
        ID = $"{SceneManager.GetActiveScene().name}_{id.Trim()}";
    }
}
