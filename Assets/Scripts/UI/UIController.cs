using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class UIController : MonoBehaviour
{
    public UIView extendMenu;
    public UIView dynamicObjectMenu;
    public UIView flowerBedDataPanel;
    public UIView tutoView;
    public UIView keyBindsBox;
    public UIView[] plantsViews;
    public UIButton gridButton;
    public UIButton cameraModeButton;
    public UIButton dataPanelInitBtn;
    public UIButton[] tmpBtn;
    public UIButtonListener uIButtonListener;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;
    public TextMeshProUGUI gardenName;
    public PlantPanelScript dataPanel;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;
    public static bool afterClickTrigger = false;

    protected Transform previewUI = null;
    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected bool subMenuOpen = true;
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
        if (this.dataPanel.GetView() != null)
            this.dataPanel.GetView().Hide();
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
        if (this.dataPanel.GetView().IsVisible)
            this.currentHideViews.Add(this.dataPanel.GetView());
    }

    public void SpawnDynMenu(GhostHandler ghost)
    {
        if (this.menu != null && this.menu.rotateState)
          return;
        SpawnMenu(ghost, this.dataPanel.GetView());
        this.menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBed flowerBed)
    {
        SpawnFlowerBedMenu(flowerBed, this.flowerBedDataPanel);
        this.flowerBedMenuScript.SetFlowerBedHandler(flowerBed);
    }

    public void SetDataPanel(string plantName, string plantType)
    {
        RectTransform menuTransform = this.extendMenu.RectTransform;
        RectTransform viewTransform = this.plantsViews[0].RectTransform;

        if (this.PlantsViewsDisplay())
            this.dataPanel.GetView().CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x - viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);


        this.dataPanel.plantName = plantName;
        this.dataPanel.plantType = plantType;


       
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
