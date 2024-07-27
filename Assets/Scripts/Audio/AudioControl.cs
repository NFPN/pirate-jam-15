using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;
using System.Linq;
using FMOD;

public class AudioControl : MonoBehaviour
{

    public static AudioControl inst;

    [Header("Volume")]
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus musicBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienEventInstance;
    private EventInstance musicEventInstance;


    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

            //musicBus = RuntimeManager.GetBus("bus:/Music");
            //musicBus = RuntimeManager.GetBus("bus:/SFX");
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        eventInstances = new();
        eventEmitters = new();

        if (DataControl.inst)
            DataControl.inst.OnLoaded += CleanUp;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVolumes()
    {
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(SFXVolume);
    }

    public void InitAmbience(EventReference ambientEventRef)
    {
        ambienEventInstance = CreateInstance(ambientEventRef);
        ambienEventInstance.start();
    }

    public void InitMusic(EventReference musicEventRef)
    {
        musicEventInstance = CreateInstance(musicEventRef);
        musicEventInstance.start();
    }

    public void PlayOneShot(Utils.SoundType sound, Vector3 worldPos = default)
    {
        RuntimeManager.PlayOneShot(GetEventReference(sound), worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        var eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterObject)
    {
        var emitter = emitterObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private EventReference GetEventReference(Utils.SoundType type) => AudioEvents.inst.audioReferences.First(x => x.type == type).reference;

    private void CleanUp()
    {
        foreach (var eventInst in eventInstances)
        {
            eventInst.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInst.release();
        }

        foreach (var eventEmitter in eventEmitters)
        {
            eventEmitter.Stop();
        }

        eventInstances.Clear();
        eventEmitters.Clear();
    }
}
