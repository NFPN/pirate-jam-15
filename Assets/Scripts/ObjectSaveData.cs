using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSaveData : MonoBehaviour
{

    public Guid ID { get; private set; }

    private void Awake()
    {
        ID = Guid.NewGuid();
    }
}
