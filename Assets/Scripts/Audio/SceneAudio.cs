using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudio : MonoBehaviour
{
    public EventReference music;

    //[Header("Parameters")]
    //public List<Utils.AudioParameters> musicParams;

    // Start is called before the first frame update
    void Start()
    {
        AudioControl.inst.InitializeMusic(music);
    }

}
