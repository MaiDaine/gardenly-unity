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
        this.plantDataRef = new PlantData("");
        this.reactProxy = ReactProxy.instance;
    }

    private void OnDisable()
    {
        if (this.imageCoroutine != null)
            StopCoroutine(this.imageCoroutine);
    }

    // Co-routine to download plant img
    public void SetPlantImg(string plantName, string plantType, Texture img)
    {
        RawImage icon = this.GetComponentInChildren<RawImage>();
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
        RawImage icon = this.GetComponentInChildren<RawImage>();
        Animator animator = icon.GetComponentInChildren<Animator>();
        Slider[] sliders = this.GetComponentsInChildren<Slider>();
        ButtonScript script = this.GetComponentInChildren<ButtonScript>();
        TextMeshProUGUI[] labels = this.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = this.reactProxy.GetPlantsData(this.plantType, this.plantName);

        if (!this.GetView().IsVisible)
            this.GetView().Show();

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Name")
                label.text = this.plantName;
        }

        if (script != null)
            script.SetGhostType(this.plantType);

        if (tmp != null && tmp.imgUrl != null)
            this.imageCoroutine = StartCoroutine(this.reactProxy.externalData.GetTexture(tmp, tmp.imgUrl));
        else
        {
            animator.enabled = true;
            icon.texture = this.textureRef;
        }

        if (this.dataPanelInitBtn.isActiveAndEnabled)
            this.dataPanelInitBtn.ExecuteClick();
        else
            this.SetDescriptionDataPanel();
    }

    public void SetDescriptionDataPanel()
    {
        TextMeshProUGUI[] labels = this.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (tmp == null)
            tmp = this.plantDataRef;
        
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
        TextMeshProUGUI[] labels = this.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (tmp == null)
            tmp = this.plantDataRef;

            foreach (TextMeshProUGUI label in labels)
            {
                if (label.name == "Flowering")
                    label.text = this.GetMonth(tmp.floweringPeriodBegin) + "  A  " + this.GetMonth(tmp.floweringPeriodEnd);
                if (label.name == "Cutting")
                    label.text = this.GetMonth(tmp.cuttingPeriodBegin) + "  A  " + this.GetMonth(tmp.cuttingPeriodEnd);
                if (label.name == "Planting")
                    label.text = this.GetMonth(tmp.plantingPeriodBegin) + "  A  " + this.GetMonth(tmp.plantingPeriodEnd);
            }
    }

    public void SetInformationsDataPanel()
    {
        TextMeshProUGUI[] labels = this.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        Slider[] sliders = this.GetComponentsInChildren<Slider>();

        if (tmp == null)
            tmp = this.plantDataRef;
            
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
                        label.text = "Absent";
                }
                if (label.name == "SoilType" && tmp.soilType != null)
                    label.text = tmp.soilType;
                if (label.name == "SoilPh")
                    label.text = "De : " + tmp.phRangeLow + " A " + tmp.phRangeHigh;
            }
            sliders[0].value = tmp.waterNeed;
            sliders[1].value = tmp.rusticity;
            sliders[2].value = tmp.sunNeed;
    }

    public string GetMonth(int month)
    {
        if (month < 1 || month > 12)
            return "Absent";
        return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
    }

    public UIView GetView()
    {
        UIView view = this.GetComponentInChildren<UIView>();

        if (view != null)
            return view;

        return null;
    }
}
