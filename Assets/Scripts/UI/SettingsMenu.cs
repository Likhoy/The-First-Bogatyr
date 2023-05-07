using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    private Toggle AudioToggle;

    public TMP_Dropdown resolutionDropdown;

    public AudioMixer theMixer;

    public TMP_Text mastLabel, musicLabel, sfxLabel;
    public Slider mastSlider, musicSlider, sfxSlider;

    public bool volumeOn = false;

    Resolution[] resolutions;

    void Start()
    {

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        float vol = 1f;
        theMixer.GetFloat("MasterVol", out vol);
        mastSlider.value = vol;

        theMixer.GetFloat("MusicVol", out vol);
        musicSlider.value = vol;

        theMixer.GetFloat("SFXVol", out vol);
        sfxSlider.value = vol;

        mastLabel.text = Mathf.RoundToInt(mastSlider.value * 100).ToString();
        musicLabel.text = Mathf.RoundToInt(musicSlider.value * 100).ToString();
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
    }


    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    public void SetMasterVol()
    {
        mastLabel.text = Mathf.RoundToInt(mastSlider.value * 100).ToString();

        theMixer.SetFloat("MasterVol", Mathf.Log10( mastSlider.value) * 20);

        PlayerPrefs.SetFloat("MasterVol", mastSlider.value);
    }

    public void SetMusicVol()
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value * 100).ToString();

        theMixer.SetFloat("MusicVol", Mathf.Log10(musicSlider.value) * 20);

        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
    }

    public void SetSFXVol()
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();

        theMixer.SetFloat("SFXVol", Mathf.Log10(sfxSlider.value) * 20);

        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
    }

    public void Mute()

    {
        if (volumeOn)
        {
            volumeOn = false;
            AudioListener.volume = 0;
        }
        else
        {
            volumeOn = true;
            AudioListener.volume = 1;

        }
    }
}
