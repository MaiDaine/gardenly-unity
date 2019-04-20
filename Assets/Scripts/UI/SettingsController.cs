using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Dropdown resolutionDropDown;
    public Dropdown qualityDropDown;
    public Dropdown antialiasingDropDown;   

    private Resolution[] resolutions;
    private GameSettings gameSettings;

    private void OnEnable()
    {
        this.gameSettings = new GameSettings();
        this.resolutions = Screen.resolutions;
        
        this.resolutionDropDown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        this.qualityDropDown.onValueChanged.AddListener(delegate { OnQualityChange(); });
        this.antialiasingDropDown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });

        foreach (Resolution resolution in resolutions)
        {
            this.resolutionDropDown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }


    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = this.gameSettings.antialiasing = (int)Mathf.Pow(2f, this.antialiasingDropDown.value);
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(this.resolutions[this.resolutionDropDown.value].width, this.resolutions[this.resolutionDropDown.value].height, Screen.fullScreen);
    }

    public void OnQualityChange()
    {
        QualitySettings.masterTextureLimit = this.gameSettings.quality = this.qualityDropDown.value;
    }

    public void ApplySettings()
    {
        // TODO write pref
    }
}