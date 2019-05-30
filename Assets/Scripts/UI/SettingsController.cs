using UnityEngine;
using TMPro;

// Class Link to settings panel
public class SettingsController : MonoBehaviour
{
    public TMP_Dropdown qualityDropDown;
    public TMP_Dropdown antialiasingDropDown;

    private GameSettings gameSettings;

    private void OnEnable()
    {
        gameSettings = new GameSettings();

        qualityDropDown.onValueChanged.AddListener(delegate { OnQualityChange(); });
        antialiasingDropDown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
    }


    public void OnAntialiasingChange() => QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow(2f, antialiasingDropDown.value);

    public void OnQualityChange() => QualitySettings.masterTextureLimit = gameSettings.quality = qualityDropDown.value;

    public void ApplySettings()
    {
        // TODO write pref
    }
}