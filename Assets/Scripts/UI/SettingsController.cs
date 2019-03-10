using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    private GameSettings gameSettings;
    public Dropdown resolutionDropDown;
    public Dropdown qualityDropDown;
    public Dropdown antialiasingDropDown;
    private Resolution[] resolutions;
    public Toggle fullScreen;
    public Slider volume;
    public AudioSource audioSource;

    void OnEnable()
    {
        gameSettings = new GameSettings();
        resolutions = Screen.resolutions;
        fullScreen.onValueChanged.AddListener(delegate { OnFullScreenToggle(); });
        volume.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        resolutionDropDown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        qualityDropDown.onValueChanged.AddListener(delegate { OnQualityChange(); });
        antialiasingDropDown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        audioSource.volume = 0;
        Screen.fullScreen = false;

        foreach (Resolution resolution in resolutions)
        {
            resolutionDropDown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }

    public void OnFullScreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullScreen.isOn;
    }

    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow(2f, antialiasingDropDown.value);
    }

    public void OnVolumeChange()
    {
        if (volume.value > 0 && !audioSource.isPlaying)
            audioSource.Play();
        audioSource.volume = gameSettings.volume = volume.value;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropDown.value].width, resolutions[resolutionDropDown.value].height, Screen.fullScreen);
    }

    public void OnQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.quality = qualityDropDown.value;
    }

    public void ApplySettings()
    {

    }
}
