using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class UIController : MonoBehaviour
{
    public UIView extendMenu;
    public UIView dynamicObjectMenu;
    public UIView dataPanel;
    public UIView flowerBedDataPanel;
    public UIView tutoView;
    public UIView keyBindsBox;
    public UIView[] plantsViews;
    public UIButton[] tmpBtn;
    public UIButton gridButton;
    public UIButton cameraModeButton;
    public UIButton dataPanelInitBtn;
    public UIButtonListener uIButtonListener;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;
    public static bool afterClickTrigger = false;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;
    public FlowerBedPanelScript flowerBedPanelScript;
    public TextMeshProUGUI gardenName;
    public Texture2D textureRef;

    protected Transform previewUI = null;
    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected bool subMenuOpen = true;
    protected GhostHandler ghost = null;
    protected FlowerBed flowerBed = null;
    protected string plantType;
    protected string plantName;
    protected PlantData dataRef;
    protected List<UIView> currentHideViews = new List<UIView>();

    private ReactProxy reactProxy;

    private void Awake()
    {
        if (this.cameraModeButton != null)
        {
            this.cameraModeButton.SelectButton();
            this.cameraModeButton.DisableButton();
        }
    }

    private void Start()
    {
        reactProxy = ReactProxy.instance;
        this.dataRef = new PlantData("Inconnu");
    }

    private void SpawnMenu(GhostHandler selectable, UIView menuType)
    {
        menuType.Show();
        this.menu = menuType.GetComponent<MenuScript>();
        menuOpen = true;
    }

    private void SpawnFlowerBedMenu(FlowerBed flowerBed, UIView menuType)
    { 
        this.flowerBedMenuScript = menuType.GetComponent<MenuFlowerBedScript>();
        flowerBedMenuOpen = true;
    }

    //TODO UI
    public void OnActionSetUpdate()
    {
        //update visual
        //revertActionSet.items
        //redoActionSet.items
    }

    public void SetCurrentHideViews(UIView view)
    {
        this.currentHideViews.Add(view);
    }

    public List<UIView> GetCurrentHideView()
    {
        return this.currentHideViews;
    }

    public bool GridButtonIsTrigger()
    {
        LabelScript tmp = this.gridButton.GetComponentInChildren<LabelScript>();
        if (tmp != null)
            return tmp.pressed;
        return false;
    }

    public void ResetButton()
    {
        foreach (UIButton btn in this.tmpBtn)
        {
            if (!btn.IsSelected)
            {
                LabelScript[] tmp = btn.GetComponentsInChildren<LabelScript>();
                foreach (LabelScript script in tmp)
                {
                    script.ResetColor();
                }
            }
        }
    }

    public void ForceResetButton()
    {
        foreach (UIButton btn in this.tmpBtn)
        {
            LabelScript[] tmp = btn.GetComponentsInChildren<LabelScript>();
            foreach (LabelScript script in tmp)
            {
                script.ResetColor();
            }
        }
    }

    public bool PlantsViewsDisplay()
    {
        foreach (UIView view in plantsViews)
        {
            if (view.IsVisible)
                return true;
        }
        return false;
    }

    public void HideViews()
    {
        foreach (UIView view in plantsViews)
        {
            if (view.IsVisible)
                view.Hide();   
        }
    }

    public FlowerBed GetFlowerBed()
    {
        return this.flowerBed;
    }

    public GhostHandler GetGhost()
    {
        return this.ghost;
    }

    public void Cancel(bool spawn = false)
    {
        if (this.menu != null)
            this.menu.DestroyMenu(spawn);
        else
            this.DestroyMenu(spawn);
        if (this.flowerBedMenuScript != null)
            this.flowerBedMenuScript.DestroyMenu();
        this.ResetButton();
    }

    public Transform GetPreviewUI() { return this.previewUI; }

    public MenuScript GetMenuScript() { return this.menu; }

    public MenuFlowerBedScript GetFlowerBedMenuScript() { return this.flowerBedMenuScript; }

    public void DestroyMenu(bool spawn = false)
    {
        if (this.dynamicObjectMenu.IsVisible)
            this.dynamicObjectMenu.Hide();
        this.uIButtonListener.GetComponentInChildren<ViewController>().ResetButtons();
        
        foreach (UIView view in this.plantsViews)
        {
     
            if (view.IsVisible)
            {
                view.Hide();
            }
        }
        if (this.dataPanel.IsVisible)
            this.dataPanel.Hide();
        if (this.tutoView.IsVisible)
            this.tutoView.Hide();
        if (this.keyBindsBox.IsVisible)
            this.keyBindsBox.Hide();
    }

    public void SaveViews()
    {
        foreach (UIView view in this.plantsViews)
        {

            if (view.IsVisible && view.name != "TutoBox")
                this.currentHideViews.Add(view);
        }
        if (this.dataPanel.IsVisible)
            this.currentHideViews.Add(this.dataPanel);
    }

    public void SpawnDynMenu(GhostHandler ghost, UIView typeMenu)
    {
        if (this.menu != null && this.menu.rotateState)
          return;
        SpawnMenu(ghost, typeMenu);
        this.menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBed flowerBed)
    {
        SpawnFlowerBedMenu(flowerBed, this.flowerBedDataPanel);
        this.flowerBedMenuScript.SetFlowerBedHandler(flowerBed);
    }

    public void SetDefaultDescriptionDataPanel(TextMeshProUGUI[] labels)
    {
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Description" && this.dataRef.description != null)
            {
                label.text = this.dataRef.description;
            }
            if (label.name == "Advices" && this.dataRef.maintainAdvice != null)
            {
                label.text = this.dataRef.maintainAdvice;
            }
        } 
    }

    public void SetDescriptionDataPanel()
    {
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (tmp != null)
        {
            foreach (TextMeshProUGUI label in labels)
            {
                if (label.name == "Description" && tmp.description != null)
                {
                    label.text = tmp.description;
                }
                if (label.name == "Advices" && tmp.maintainAdvice != null)
                {
                    label.text = tmp.maintainAdvice;
                }
            }
        }
        else
            this.SetDefaultDescriptionDataPanel(labels);
    }

    public string GetMonth(int month)
    {
        switch (month)
        {
            case 0:
                return "Absent";
            case 1:
                return "Janvier";
            case 2:
                return "Février";
            case 3:
                return "Mars";
            case 4:
                return "Avril";
            case 5:
                return "Mai";
            case 6:
                return "Juin";
            case 7:
                return "Juillet";
            case 8:
                return "Août";
            case 9:
                return "Septembre";
            case 10:
                return "Octobre";
            case 11:
                return "Novembre";
            case 12:
                return "Décembre";
            default:
                break;
        }
        return "";
    }

    public void SetDefaultMaintainDataPanel(TextMeshProUGUI[] labels)
    {
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Flowering")
            {
                label.text = "Absent" + "  A  " + "Absent";
            }
            if (label.name == "Cutting")
            {
                label.text = "Absent" + "  A  " + "Absent";
            }
            if (label.name == "Planting")
            {
                label.text = "Absent" + "  A  " + "Absent";
            }
        }
    }

    public void SetMaintainDataPanel()
    {
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        if (tmp != null)
        {
            foreach (TextMeshProUGUI label in labels)
            {
                if (label.name == "Flowering" )
                {
                    label.text = this.GetMonth(tmp.floweringPeriodBegin) + "  A  " + this.GetMonth(tmp.floweringPeriodEnd);
                }
                if (label.name == "Cutting")
                {
                    label.text = this.GetMonth(tmp.cuttingPeriodBegin) + "  A  " + this.GetMonth(tmp.cuttingPeriodEnd);
                }
                if (label.name == "Planting")
                {
                    label.text = this.GetMonth(tmp.plantingPeriodBegin) + "  A  " + this.GetMonth(tmp.plantingPeriodEnd);
                }
            }
        }
        else
            this.SetDefaultMaintainDataPanel(labels);
    }
    public void SetInformationsDataPanelDefault(TextMeshProUGUI[] labels, Slider[] sliders)
    {
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "HeightMin")
            {

                label.text = this.dataRef.heightMin + "cm";
            }
            if (label.name == "HeightMax")
            {
                label.text = this.dataRef.heightMax + "cm";
            }
            if (label.name == "Shape")
            {
                label.text = "Absent";
            }
            if (label.name == "Colors" && this.dataRef.plantColor != null)
            {
                label.text = "absent";
            }
            if (label.name == "SoilType" && this.dataRef.soilType != null)
            {
                label.text = this.dataRef.soilType;
            }
            if (label.name == "SoilPh")
            {
                label.text = "De : " + this.dataRef.phRangeLow + " A " + this.dataRef.phRangeHigh;
            }
        }

        sliders[0].value = this.dataRef.waterNeed;
        sliders[1].value = this.dataRef.rusticity;
        sliders[2].value = this.dataRef.sunNeed;
    }

    public void SetInformationsDataPanel()
    {
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();

        if (tmp != null)
        {
            foreach (TextMeshProUGUI label in labels)
            {
                if (label.name == "HeightMin")
                {

                    label.text = tmp.heightMin + "cm";
                }
                if (label.name == "HeightMax")
                {
                    label.text = tmp.heightMax + "cm";
                }
                if (label.name == "Shape" && tmp.shape != null)
                {
                    label.text = tmp.shape;
                }
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
                {
                    label.text = tmp.soilType;
                }
                if (label.name == "SoilPh")
                {
                    label.text = "De : " + tmp.phRangeLow + " A " + tmp.phRangeHigh;
                }
            }

            sliders[0].value = tmp.waterNeed;
            sliders[1].value = tmp.rusticity;
            sliders[2].value = tmp.sunNeed;
        }
        else
            this.SetInformationsDataPanelDefault(labels, sliders);
    }

    public void SetPlantImg(string plantName, string plantType, Texture img)
    {
        RawImage icon = this.dataPanel.GetComponentInChildren<RawImage>();
        Animator animator = icon.GetComponentInChildren<Animator>();

        if (icon != null && img != null)
        {
            if (animator != null)
                animator.enabled = false;
            icon.texture = img;
            icon.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void SetDataPanel(string plantName, string plantType)
    {
        RawImage icon = this.dataPanel.GetComponentInChildren<RawImage>();
        Animator animator = icon.GetComponentInChildren<Animator>();
        RectTransform menuTransform = this.extendMenu.RectTransform;
        RectTransform viewTransform = this.plantsViews[0].RectTransform;
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();


        this.plantName = plantName;
        this.plantType = plantType;
        if (labels[labels.Length - 1].text == plantName && this.dataPanel.IsVisible)
        {
            this.dataPanel.Hide();
            return;
        }

        if (this.PlantsViewsDisplay())
            this.dataPanel.CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x - viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);

        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();
        ButtonScript[] script = this.dataPanel.GetComponentsInChildren<ButtonScript>();

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Name")
                label.text = plantName;
        }
        
        if (tmp != null && tmp.imgUrl != null)
            StartCoroutine(this.reactProxy.externalData.GetTexture(tmp, tmp.imgUrl));
        else
        {
            animator.enabled = true;
            icon.texture = this.textureRef;
        }

        if (script[0] != null)
            script[0].SetGhost(plantType);
        if (!this.dataPanel.IsVisible)
        {
            this.dataPanel.Show();
        }
        if (tmp != null)
            this.dataPanelInitBtn.ExecuteClick();
    }

    public void SetFlowerBedDataPanel(FlowerBed flowerBedRef)
    {
        TextMeshProUGUI[] texts = this.flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        TMP_Dropdown type = this.flowerBedDataPanel.GetComponentInChildren<TMP_Dropdown>();

        SpawnFlowerBedMenu(flowerBedRef, this.flowerBedDataPanel);
        this.flowerBedMenuScript.SetFlowerBedHandler(flowerBedRef);
        if (!this.flowerBedDataPanel.IsVisible)
             this.flowerBedDataPanel.Show();
        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt.name == "Name")
                txt.text = flowerBedRef.flowerBedName;
        }
        if (flowerBedRef.soilType != "PLACEHOLDER")
        {
            foreach (TMP_Dropdown.OptionData data in type.options)
            {
                if (data.text == flowerBedRef.soilType)
                    type.value = type.options.IndexOf(data);
            }
        }
        this.flowerBed = flowerBedRef;
    }

    public void ResetFlowerBedDataPanel()
    {
        this.flowerBed.name = "";
        this.flowerBed.soilType = "";
    }

    public void UpdateNameFlowerBed(string updateName)
    {
        TextMeshProUGUI[] texts = this.flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt.name == "Name")
                txt.text = updateName;
        }
        
        this.flowerBed.flowerBedName = updateName;
    }

    public void UpdateTypeFlowerBed(string updateType)
    {        
        this.flowerBed.soilType = updateType;
    }
}
