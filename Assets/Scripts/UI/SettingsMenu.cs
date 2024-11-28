using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Diagnostics.Contracts;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private GameObject controlsSettingsPanel;   

    [SerializeField] private Toggle fulscreeenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    [SerializeField] private AudioMixer theMixer;
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private TMP_Text mastLabel, musicLabel, sfxLabel;
    [SerializeField] private Slider mastSlider, musicSlider, sfxSlider;

    [SerializeField] private Slider gammaSlider;
    [SerializeField] private TMP_Text gammaLabel;
    [SerializeField] private Slider contrastSlider;
    [SerializeField] private TMP_Text contrastLabel;
    [SerializeField] private VolumeProfile profile;
    private LiftGammaGain volumeGain;
    private ColorAdjustments volumeContrast;

    void Start()
    {
        InitializeFullscreenToggleSettings();
        InitializeResolutionSettings();
        InitializeGammaSettings();
        InitialozeContrastSettings();
        InitializeAudioToggleSettings();
        InitializeAudioSlidersSettings();
    }

    private void InitializeFullscreenToggleSettings()
    {
        fulscreeenToggle.onValueChanged.AddListener(SetFullscreen);

        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            fulscreeenToggle.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;
        }
        else
        {
            fulscreeenToggle.isOn = true;
        }
    }

    private void InitializeResolutionSettings()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Sort((a, b) =>
        {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height && (float)filteredResolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        SetResolution(currentResolutionIndex);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void InitializeGammaSettings()
    {        
        profile.TryGet<LiftGammaGain>(out volumeGain);

        gammaSlider.onValueChanged.AddListener(SetGamma);

        if (PlayerPrefs.HasKey("GammaValue"))
        {
            float gammaValue = PlayerPrefs.GetFloat("GammaValue");
            gammaSlider.value = gammaValue;
        }
        else
        {
            gammaSlider.value = volumeGain.gamma.value.x;
        }
    }

    private void InitialozeContrastSettings()
    {
        profile.TryGet<ColorAdjustments>(out volumeContrast);

        contrastSlider.onValueChanged.AddListener(SetContrast);

        if (PlayerPrefs.HasKey("ContrastValue"))
        {
            float contrastValue = PlayerPrefs.GetFloat("ContrastValue");
            contrastSlider.value = contrastValue;
        }
        else
        {
            contrastSlider.value = volumeContrast.contrast.value;
        }
    }

    private void InitializeAudioToggleSettings()
    {
        audioToggle.onValueChanged.AddListener(SetVolumeMute);

        if (PlayerPrefs.HasKey("VolumeMute"))
        {
            audioToggle.isOn = PlayerPrefs.GetInt("VolumeMute") == 1;
        }
        else
        {
            audioToggle.isOn = true;
        }
    }

    private void InitializeAudioSlidersSettings()
    {
        mastSlider.onValueChanged.AddListener(SetMasterVol);
        musicSlider.onValueChanged.AddListener(SetMusicVol);
        sfxSlider.onValueChanged.AddListener(SetSFXVol);

        if (PlayerPrefs.HasKey("MasterVol"))
        {
            mastSlider.value = PlayerPrefs.GetFloat("MasterVol");
        }
        else
        {
            mastSlider.value = 1f;
        }

        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        }
        else
        {
            musicSlider.value = 0f;
        }

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
        }
        else
        {
            sfxSlider.value = 0f;
        }

        mastLabel.text = Mathf.RoundToInt(mastSlider.value * 100).ToString();
        musicLabel.text = Mathf.RoundToInt(musicSlider.value * 100).ToString();
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void SetMasterVol(float value)
    {
        mastLabel.text = Mathf.RoundToInt(mastSlider.value * 100).ToString();

        theMixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);

        PlayerPrefs.SetFloat("MasterVol", value);
    }

    public void SetMusicVol(float value)
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value * 100).ToString();

        theMixer.SetFloat("MusicVol", Mathf.Log10(musicSlider.value) * 20);

        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
    }

    public void SetSFXVol(float value)
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();

        theMixer.SetFloat("SFXVol", Mathf.Log10(sfxSlider.value) * 20);

        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetVolumeMute(bool isMute)
    {
        if (isMute)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }

        PlayerPrefs.SetInt("VolumeMute", isMute ? 1 : 0);
    }

    public void SetGamma(float value)
    {
        if (volumeGain != null)
        {
            volumeGain.gamma.value = new Vector4(value, value, value, value);            
            PlayerPrefs.SetFloat("GammaValue", value);

            gammaLabel.text = $"{value + 1:F2}";
        }
    }

    private void SetContrast(float value)
    {
        if (volumeContrast != null)
        {
            volumeContrast.contrast.value = value; 
        }

        PlayerPrefs.SetFloat("ContrastValue", value);

        contrastLabel.text = $"{value:F0}";
    }

    public void HideSettings()
    {
        PlayerPrefs.Save();

        audioSettingsPanel.SetActive(false);
        videoSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }
}
