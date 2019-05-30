using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;
using TMPro;

// Manage the plants panel
public class PlantPanelScript : MonoBehaviour
{
    public string plantName;
    public string plantType;
    public Texture2D textureRef;
    public UIButton dataPanelInitBtn;

    private PlantData plantDataRef;
    private ReactProxy reactProxy;
    private Coroutine imageCoroutine;

    private void Start()
    {
        plantDataRef = new PlantData("");
        reactProxy = ReactProxy.instance;
    }

    private void OnDisable()
    {
        if (imageCoroutine != null)
            StopCoroutine(imageCoroutine);
    }

    // Co-routine to download plant img
    public void SetPlantImg(string plantName, string plantType, Texture img)
    {
        RawImage icon = GetComponentInChildren<RawImage>();
        Animator animator = icon.GetComponentInChildren<Animator>();

        if (icon != null && img != null)
        {
            if (animator != null)
                animator.enabled = false;
            icon.texture = img;
            icon.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    // Set data of the panel and show it, plantDataRef if Graphql request return null.
    public void SetData()
    {
        RawImage icon = GetComponentInChildren<RawImage>();
        Animator animator = icon.GetComponentInChildren<Animator>();
        Slider[] sliders = GetComponentsInChildren<Slider>();
        ButtonScript script = GetComponentInChildren<ButtonScript>();
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (!GetView().IsVisible)
            GetView().Show();

        plantDataRef.SetDefaultData();
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Name")
                label.text = plantName;
        }

        if (script != null)
            script.SetGhostType(plantType);

        if (tmp != null && tmp.imgUrl != null)
            imageCoroutine = StartCoroutine(reactProxy.externalData.GetTexture(tmp, tmp.imgUrl));
        else
        {
            animator.enabled = true;
            icon.texture = textureRef;
        }

        if (dataPanelInitBtn.isActiveAndEnabled)
            dataPanelInitBtn.ExecuteClick();
        else
            SetDescriptionDataPanel();
    }

    public void SetDescriptionDataPanel()
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (tmp == null)
            tmp = plantDataRef;

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Description" && tmp.description != null)
                label.text = tmp.description;
            if (label.name == "Advices" && tmp.maintainAdvice != null)
                label.text = tmp.maintainAdvice;
        }

    }

    public void SetMaintainDataPanel()
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (tmp == null)
            tmp = plantDataRef;

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Flowering")
                label.text = GetMonth(tmp.floweringPeriodBegin) + LocalisationController.instance.GetText("plant_data", "to") + GetMonth(tmp.floweringPeriodEnd);
            if (label.name == "Cutting")
                label.text = GetMonth(tmp.cuttingPeriodBegin) + LocalisationController.instance.GetText("plant_data", "to") + GetMonth(tmp.cuttingPeriodEnd);
            if (label.name == "Planting")
                label.text = GetMonth(tmp.plantingPeriodBegin) + LocalisationController.instance.GetText("plant_data", "to") + GetMonth(tmp.plantingPeriodEnd);
        }
    }

    public void SetInformationsDataPanel()
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        Slider[] sliders = GetComponentsInChildren<Slider>();

        if (tmp == null)
            tmp = plantDataRef;

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "HeightMin")
                label.text = tmp.heightMin + "cm";
            if (label.name == "HeightMax")
                label.text = tmp.heightMax + "cm";
            if (label.name == "Shape" && tmp.shape != null)
                label.text = tmp.shape;
            if (label.name == "Colors")
            {
                if (tmp.plantColor != null)
                {
                    label.text = "";
                    foreach (string color in tmp.plantColor)
                    {
                        label.text = label.text + color + " ";
                    }
                }
                else
                    label.text = LocalisationController.instance.GetText("plant_data", "missing");
            }
            if (label.name == "SoilType" && tmp.soilType != null)
                label.text = tmp.soilType;
            if (label.name == "SoilPh")
                label.text = LocalisationController.instance.GetText("plant_data", "from") + tmp.phRangeLow + LocalisationController.instance.GetText("plant_data", "to") + tmp.phRangeHigh;
        }
        sliders[0].value = tmp.waterNeed;
        sliders[1].value = tmp.rusticity;
        sliders[2].value = tmp.sunNeed;
    }

    public string GetMonth(int month)
    {
        if (month < 1 || month > 12)
            return LocalisationController.instance.GetText("plant_data", "missing");
        return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
    }

    public UIView GetView()
    {
        UIView view = GetComponentInChildren<UIView>();

        return view;
    }
}
