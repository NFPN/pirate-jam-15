using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSaveData : MonoBehaviour
{
    [SerializeField]private string id;

    private string _id;
    public string ID { get => _id; set => _id = $"{SceneManager.GetActiveScene().name}_{value.Trim()}"; }

    private void Awake()
    {
        ID = id;
    }

}
