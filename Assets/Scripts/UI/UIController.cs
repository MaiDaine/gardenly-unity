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
    public UIButton cameraModeButton;
    public UIButtonListener uIButtonListener;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;
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

    private void Awake()
    {
        this.gardenName.text = Camera.main.GetComponent<GardenData>().gardenName;
        if (this.cameraModeButton != null)
        {
            this.cameraModeButton.SelectButton();
            this.cameraModeButton.DisableButton();
        }
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
        if (this.flowerBedMenuScript)
            this.flowerBedMenuScript.DestroyMenu();
        this.ResetButton();
    }

    public Transform GetPreviewUI() { return this.previewUI; }

    public MenuScript GetMenuScript() { return this.menu; }

    public MenuFlowerBedScript GetFlowerBedMenuScript() { return this.flowerBedMenuScript; }

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


    public void SetDataPanel(string plantName, string plantType)
    {
        RectTransform menuTransform = this.extendMenu.RectTransform;
        RectTransform viewTransform = this.plantsViews[0].RectTransform;

        if (this.dataPanel.GetComponentInChildren<TextMeshProUGUI>().text == plantName && this.dataPanel.IsVisible)
        {
            this.dataPanel.Hide();
            return;
        }

        if (this.PlantsViewsDisplay())
            this.dataPanel.CustomStartAnchoredPosition = new Vector3(- menuTransform.sizeDelta.x - viewTransform.sizeDelta.x + 0.3f, -33.46f, 0);

        PlantData tmp = ReactProxy.instance.externalData.plants[plantType][plantName];
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();
        RawImage icon = this.dataPanel.GetComponentInChildren<RawImage>();
        ButtonScript[] script = this.dataPanel.GetComponentsInChildren<ButtonScript>();

        if (tmp == null)
            return;
       /* if (labels != null && labels.Length > 0)
        {
            labels[0].text = tmp.name;
            labels[1].text = tmp.description;
            labels[11].text = tmp.phRangeLow + ", " + tmp.phRangeHigh;
        }
        if (sliders != null && sliders.Length > 0)
        {
            sliders[0].value = tmp.waterNeed;
            sliders[1].value = tmp.sunNeed;
            sliders[2].value = tmp.rusticity;
        }
        if (icon != null && tmp.image != null)
        {
            icon.texture = tmp.image;
        }
        if (tmp.image == null)
            icon.texture = this.textureRef;
        if (script[0] != null)
            script[0].SetGhost(plantType);*/
        if (!this.dataPanel.IsVisible)
        {
            this.dataPanel.Show();
        }
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