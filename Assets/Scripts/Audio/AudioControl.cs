using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;

public class AudioControl : MonoBehaviour
{
    private List<EventInstance> eventInstances;

    private EventInstance ambienEventInstance;

    private void Start()
    {
        eventInstances = new();

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitAmbience(EventReference ambientEventRef)
    {
        
    }
}
