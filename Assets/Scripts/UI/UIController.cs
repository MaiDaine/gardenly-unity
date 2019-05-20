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
    protected List<UIView> currentHideViews = new List<UIView>();

    private ReactProxy reactProxy;

    private void Awake()
    {
        //this.gardenName.text = ReactProxy.instance.GetComponent<GardenData>().gardenName;
        if (this.cameraModeButton != null)
        {
            this.cameraModeButton.SelectButton();
            this.cameraModeButton.DisableButton();
        }
    }

    private void Start()
    {
        reactProxy = ReactProxy.instance;   
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

    public void SetDescriptionDataPanel()
    {
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        
        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Description" && tmp.description != null)
            {
                label.text = tmp.description;
            }
            if (label.name == "Advices" && tmp.description != null)
            {

            }
        }
    }

    public string GetMonth(int month)
    {
        switch (month)
        {
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

    public void SetMaintainDataPanel()
    {
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "Flowering" && tmp.floweringPeriodBegin != 0 && tmp.floweringPeriodEnd != 0)
            {
                label.text = this.GetMonth(tmp.floweringPeriodBegin) + "  A  " + this.GetMonth(tmp.floweringPeriodEnd);
            }
            if (label.name == "Cutting" && tmp.cuttingPeriodBegin != 0 && tmp.cuttingPeriodEnd != 0)
            {
                label.text = this.GetMonth(tmp.cuttingPeriodBegin) + "  A  " + this.GetMonth(tmp.cuttingPeriodEnd);
            }
            if (label.name == "Planting" && tmp.plantingPeriodBegin != 0 && tmp.plantingPeriodEnd != 0)
            {
                label.text = this.GetMonth(tmp.plantingPeriodBegin) + "  A  " + this.GetMonth(tmp.plantingPeriodEnd);
            }
        }
    }

    public void SetInformationsDataPanel()
    {
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();

        foreach (TextMeshProUGUI label in labels)
        {
            if (label.name == "HeightMin" && tmp.heightMin != 0)
            {
                label.text = tmp.heightMin + "cm";
            }
            if (label.name == "HeightMax" && tmp.heightMax != 0)
            {
                label.text = tmp.heightMax + "cm";
            }
            if (label.name == "Shape" && tmp.shape != null)
            {
                label.text = tmp.shape;
            }
            if (label.name == "Colors" && tmp.plantColor != null)
            {
                label.text = tmp.plantColor;
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
        if (img == null)
        {
            animator.enabled = true;
            icon.texture = this.textureRef;
        }
    }

    public void SetDataPanel(string plantName, string plantType)
    {
        RectTransform menuTransform = this.extendMenu.RectTransform;
        RectTransform viewTransform = this.plantsViews[0].RectTransform;
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();

        this.plantName = plantName;
        this.plantType = plantType;

        if (labels[labels.Length - 1].text == this.plantName && this.dataPanel.IsVisible)
        {
            this.dataPanel.Hide();
            return;
        }

        if (this.PlantsViewsDisplay())
            this.dataPanel.CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x - viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);

        PlantData tmp = reactProxy.GetPlantsData(plantType, plantName);
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();
        ButtonScript[] script = this.dataPanel.GetComponentsInChildren<ButtonScript>();
        if (tmp == null)
            return;

        foreach(TextMeshProUGUI label in labels)
        {
            if (label.name == "Name")
            {
                label.text = tmp.name;
            }
               
        }
        StartCoroutine(this.reactProxy.externalData.GetTexture(tmp, tmp.imgUrl));
      
        if (script[0] != null)
            script[0].SetGhost(plantType);
        if (!this.dataPanel.IsVisible)
        {
            this.dataPanel.Show();
        }
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
