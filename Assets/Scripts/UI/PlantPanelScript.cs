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
        reactProxy = ReactProxy.instance;
    }

    private void OnDisable()
    {
        if (imageCoroutine != null)
            StopCoroutine(imageCoroutine);
    }

    // Co-routine to download plant img
    /*  public void SetPlantImg(string plantName, string plantType, Texture img)
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
      } */

    private void OnPictureLoaded(Texture2D texture)
    {
        RawImage icon = GetComponentInChildren<RawImage>();
        Animator animator = icon.GetComponentInChildren<Animator>();

        textureRef = texture;
        animator.enabled = false;
        icon.transform.eulerAngles = new Vector3(0, 0, 0);
        icon.texture = textureRef;
    }

    private void OnDataLoaded(PlantData plantData)
    {
        plantDataRef = plantData;
        if (plantDataRef.image == null)
            imageCoroutine = StartCoroutine(reactProxy.externalData.GetTexture(plantData, plantDataRef.imgUrl));

        SetDescriptionDataPanel();
        SetMaintainDataPanel();
        SetInformationsDataPanel();
    }

    private void FinalizeView(TextMeshProUGUI[] labels, ButtonScript dynButtonscript)
    {
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Name")
                label.text = plantDataRef.name;
        }

        if (dynButtonscript != null)
            dynButtonscript.SetGhostType(plantDataRef.type);

        if (dataPanelInitBtn.isActiveAndEnabled)
            dataPanelInitBtn.ExecuteClick();
    }

    // Set data of the panel and show it, plantDataRef if Graphql request return null.
    public void SetData(string plantType, string plantName)
    {
        ButtonScript script = GetComponentInChildren<ButtonScript>();
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();

        if (!GetView().IsVisible)
            GetView().Show();

        reactProxy.externalData.callbackLoadData[plantName] = OnDataLoaded;
        reactProxy.externalData.callbackFinishDownloadImage[plantName] = OnPictureLoaded;

        PlantData fetchData = reactProxy.GetPlantsData(plantType, plantName);

        if (fetchData == null && (plantDataRef == null || plantDataRef.name != plantName))
        {
            plantDataRef = new PlantData("");

            plantDataRef.name = plantName;
            plantDataRef.type = plantType;
            plantDataRef.SetDefaultData();
        }

        if (fetchData != null)
        {
            Debug.Log("DATA NOT NULL " + fetchData.name + " " + fetchData.waterNeed);
            plantDataRef = fetchData;
        }

        SetDescriptionDataPanel();
        SetMaintainDataPanel();
        SetInformationsDataPanel();

        FinalizeView(labels, script);
    }

    public void SetDescriptionDataPanel()
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Description" && plantDataRef.description != null)
                label.text = plantDataRef.description;
            if (label.name == "Advices" && plantDataRef.maintainAdvice != null)
                label.text = plantDataRef.maintainAdvice;
        }

    }

    public void SetMaintainDataPanel()
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Flowering")
                label.text = GetMonth(plantDataRef.floweringPeriodBegin) + LocalisationController.instance.GetText("plant_data", "to") + GetMonth(plantDataRef.floweringPeriodEnd);
            if (label.name == "Cutting")
                label.text = GetMonth(plantDataRef.cuttingPeriodBegin) + LocalisationController.instance.GetText("plant_data", "to") + GetMonth(plantDataRef.cuttingPeriodEnd);
            if (label.name == "Planting")
                label.text = GetMonth(plantDataRef.plantingPeriodBegin) + LocalisationController.instance.GetText("plant_data", "to") + GetMonth(plantDataRef.plantingPeriodEnd);
        }
    }

    public void SetInformationsDataPanel()
    {
        TextMeshProUGUI[] labels = GetComponentsInChildren<TextMeshProUGUI>();
        Slider[] sliders = GetComponentsInChildren<Slider>();

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "HeightMin")
                label.text = plantDataRef.heightMin + "cm";
            else if (label.name == "HeightMax")
                label.text = plantDataRef.heightMax + "cm";
            else if (label.name == "Shape" && plantDataRef.shape != null)
                label.text = plantDataRef.shape;
            else if (label.name == "Colors")
            {
                if (plantDataRef.plantColor != null)
                {
                    label.text = "";
                    foreach (string color in plantDataRef.plantColor)
                    {
                        label.text = label.text + color + " ";
                    }
                }
                else
                    label.text = LocalisationController.instance.GetText("plant_data", "missing");
            }
            else if (label.name == "SoilType" && plantDataRef.soilType != null)
                label.text = plantDataRef.soilType;
            else if (label.name == "SoilPh")
                label.text = LocalisationController.instance.GetText("plant_data", "from") + plantDataRef.phRangeLow + LocalisationController.instance.GetText("plant_data", "to") + plantDataRef.phRangeHigh;
        }
        if (sliders.Length == 3)
        {
            sliders[0].value = plantDataRef.waterNeed;
            sliders[1].value = plantDataRef.rusticity;
            sliders[2].value = plantDataRef.sunNeed;
        }
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
