using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    public void PlayButtonPressedAudio()
    {
        AudioControl.inst.PlayOneShot(Utils.SoundType.UIClickBig);
    }
}
