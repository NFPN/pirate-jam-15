using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEvents : MonoBehaviour
{
    [field: SerializeField] public EventReference WorldChange { get; private set; }
    [field: SerializeField] public EventReference Dash { get; private set; }
    [field: SerializeField] public EventReference Fireball { get; private set; }
    [field: SerializeField] public EventReference Explosion { get; private set; }
    [field: SerializeField] public EventReference AOEMagic { get; private set; }

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