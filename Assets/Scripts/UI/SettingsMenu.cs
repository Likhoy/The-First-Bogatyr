using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer am;

    public void AudioVolume(float sliderValue)
    {
        am.SetFloat("Volume", sliderValue);
    }
}
