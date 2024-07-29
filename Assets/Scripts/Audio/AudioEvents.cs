using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvents : MonoBehaviour
{
    [field: Header("SFX")]
    [field: SerializeField] public List<AudioReference> audioReferences {  get; private set; }

    public static AudioEvents inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}

[System.Serializable]
public struct AudioReference
{
    public Utils.SoundType type;
    public EventReference reference;
}