using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public TMPro.TMP_Dropdown qualityDropDown;
    public TMPro.TMP_Dropdown antialiasingDropDown;

    private GameSettings gameSettings;

    private void OnEnable()
    {
        this.gameSettings = new GameSettings();
        
        this.qualityDropDown.onValueChanged.AddListener(delegate { OnQualityChange(); });
        this.antialiasingDropDown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
    }


    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = this.gameSettings.antialiasing = (int)Mathf.Pow(2f, this.antialiasingDropDown.value);
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