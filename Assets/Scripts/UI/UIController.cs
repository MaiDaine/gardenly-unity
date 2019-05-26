using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Doozy.Engine.UI;

public class UIController : MonoBehaviour
{
    public UIView extendMenu;
    public UIView dynamicObjectMenu;
    public UIView flowerBedDataPanel;
  //  public UIView tutoView;
    public UIView[] plantsViews;
    public UIButton gridButton;
    public UIButton cameraModeButton;
    public UIButton[] tmpBtn;
    public UIButtonListener uIButtonListener;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;
    public TextMeshProUGUI gardenName;
    public PlantPanelScript dataPanel;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;

    //protected Transform previewUI = null;
    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected GhostHandler ghost = null;
    protected FlowerBed flowerBed = null;
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
        this.gardenName.text = reactProxy.GetComponent<GardenData>().gardenName;
    }


    // Manage show / hide views
    public void SpawnDynMenu(GhostHandler ghost)
    {
        if (this.menu != null && this.menu.rotateState)
            return;
        this.dynamicObjectMenu.Show();
        this.menu = this.dynamicObjectMenu.GetComponent<MenuScript>();
        menuOpen = true;
        this.menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBed flowerBed)
    {
        this.flowerBedMenuScript = this.flowerBedDataPanel.GetComponent<MenuFlowerBedScript>();
        flowerBedMenuOpen = true;
        this.flowerBedMenuScript.SetFlowerBedHandler(flowerBed);
    }

    public void Cancel(bool spawn = false)
    {
        if (this.menu != null)
            this.menu.DestroyMenu(spawn);
        else
            this.DestroyMenu();
        if (this.flowerBedMenuScript != null)
            this.flowerBedMenuScript.DestroyMenu();
        this.ResetButton();
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

    public void DestroyMenu()
    {
        if (this.dynamicObjectMenu != null && this.dynamicObjectMenu.IsVisible)
            this.dynamicObjectMenu.Hide();
        if (this.uIButtonListener != null)
            this.uIButtonListener.GetComponentInChildren<ViewController>().ResetButtons();
        this.HideViews();
        if (this.dataPanel != null && this.dataPanel.GetView() != null)
            this.dataPanel.GetView().Hide();
    }

    //TODO UI
    public void OnActionSetUpdate()
    {
        //update visual
        //revertActionSet.items
        //redoActionSet.items
    }

    public void ResetButton(bool forced = false)
    {
        foreach (UIButton btn in this.tmpBtn)
        {
            if (!btn.IsSelected || forced)
            {
                LabelScript[] tmp = btn.GetComponentsInChildren<LabelScript>();
                foreach (LabelScript script in tmp)
                {
                    script.ResetColor();
                }
            }
        }
    }

    public void SaveViews()
    {
        this.currentHideViews.Clear();
        foreach (UIView view in this.plantsViews)
        {
            if (view.IsVisible && view.name != "TutoBox")
                this.currentHideViews.Add(view);
        }
        if (this.dataPanel.GetView().IsVisible)
            this.currentHideViews.Add(this.dataPanel.GetView());
    }

    // Plant data panel management
    public void SetDataPanel(string plantName, string plantType)
    {
        RectTransform menuTransform = this.extendMenu.RectTransform;
        RectTransform viewTransform = this.plantsViews[0].RectTransform;

        if (this.PlantsViewsDisplay())
            this.dataPanel.GetView().CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x - viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);

        if (this.dataPanel.plantName == plantName && this.dataPanel.GetView().IsVisible)
        {
            this.dataPanel.GetView().Hide();
            return;
        }

        this.dataPanel.plantName = plantName;
        this.dataPanel.plantType = plantType;

        this.dataPanel.SetData();
    }

    // FB panel management
    public void SetFlowerBedDataPanel(FlowerBed flowerBedRef)
    {
        TextMeshProUGUI[] texts = this.flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        TMP_Dropdown type = this.flowerBedDataPanel.GetComponentInChildren<TMP_Dropdown>();

        SpawnFlowerBedMenu(flowerBedRef);
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

    // utils
    public bool GridButtonIsTrigger()
    {
        LabelScript tmp = this.gridButton.GetComponentInChildren<LabelScript>();
        if (tmp != null)
            return tmp.pressed;
        return false;
    }

    public MenuScript GetMenuScript() { return this.menu; }

    public MenuFlowerBedScript GetFlowerBedMenuScript() { return this.flowerBedMenuScript; }

    public void SetCurrentHideViews(UIView view)    { this.currentHideViews.Add(view); }

    public List<UIView> GetCurrentHideView() { return this.currentHideViews; }

    public FlowerBed GetFlowerBed()     { return this.flowerBed; }

    public GhostHandler GetGhost() { return this.ghost; }
}
