using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Doozy.Engine.UI;

public class UIController : MonoBehaviour
{
    public UIView extendMenu;
    public UIView dynamicObjectMenu;
    public UIView flowerBedDataPanel;
    public UIView[] plantsViews;
    public UIButton gridButton;
    public UIButton cameraModeButton;
    public UIButton[] tmpBtn;
    public UIButtonListener uIButtonListener;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;
    public TextMeshProUGUI gardenName;
    public PlantPanelScript dataPanel;
    public UIInteractions uIInteractions = null;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;

    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected GhostHandler ghost = null;
    protected FlowerBed flowerBed = null;
    protected List<UIView> currentHideViews = new List<UIView>();

    private ReactProxy reactProxy;
    private Vector3 anchorOpenView = new Vector3();
    private Vector3 anchorCloseView = new Vector3();

    private void Awake()
    {
        if (cameraModeButton != null)
        {
            cameraModeButton.SelectButton();
            cameraModeButton.DisableButton();
        }
    }

    private void Start()
    {
        reactProxy = ReactProxy.instance;
        gardenName.text = reactProxy.GetComponent<GardenData>().gardenName;
        anchorOpenView = new Vector3(-extendMenu.RectTransform.sizeDelta.x - plantsViews[0].RectTransform.sizeDelta.x + 0.3f, -33.46f, 0);
        anchorCloseView = new Vector3(-extendMenu.RectTransform.sizeDelta.x + 0.3f, -33.46f, 0);
        uIInteractions = new UIInteractions();
        uIInteractions.Init();//TODO UI
    }


    // Manage show / hide views
    public void SpawnDynMenu(GhostHandler ghost)
    {
        if (menu != null && menu.rotateState)
            return;
        dynamicObjectMenu.Show();
        menu = dynamicObjectMenu.GetComponent<MenuScript>();
        menuOpen = true;
        menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBed flowerBed)
    {
        flowerBedMenuScript = flowerBedDataPanel.GetComponent<MenuFlowerBedScript>();
        flowerBedMenuOpen = true;
        flowerBedMenuScript.SetFlowerBedHandler(flowerBed);
    }

    public void Cancel()
    {
        if (menu != null)
            menu.DestroyMenu();
        DestroyMenu();
        ResetButton();
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
        if (dynamicObjectMenu != null && dynamicObjectMenu.IsVisible)
            dynamicObjectMenu.Hide();
        if (uIButtonListener != null)
            uIButtonListener.GetComponentInChildren<ViewController>().ResetButtons();
        HideViews();
        if (dataPanel != null && dataPanel.GetView() != null)
            dataPanel.GetView().Hide();
        if (flowerBedDataPanel != null && flowerBedDataPanel.IsVisible)
            flowerBedDataPanel.Hide();
        menuOpen = false;
        flowerBedMenuOpen = false;
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
        foreach (UIButton btn in tmpBtn)
        {
            if (btn != null && !btn.IsSelected || forced)
            {
                LabelScript[] tmp = btn.GetComponentsInChildren<LabelScript>();
                foreach (LabelScript script in tmp)
                {
                    script.ResetColor();
                }
            }
        }
    }

    // Hide and Show views after spawn plant
    public void SaveViews()
    {
        currentHideViews.Clear();
        foreach (UIView view in plantsViews)
        {
            if (view.IsVisible && view.name != "TutoBox")
                currentHideViews.Add(view);
        }
        if (dataPanel.GetView().IsVisible)
            currentHideViews.Add(dataPanel.GetView());
    }

    // Plant data panel management
    public void SetDataPanel(string plantName, string plantType)
    {

        if (PlantsViewsDisplay())
            dataPanel.GetView().CustomStartAnchoredPosition = anchorOpenView;
        else
            dataPanel.GetView().CustomStartAnchoredPosition = anchorCloseView;

        if (dataPanel.plantName == plantName && dataPanel.GetView().IsVisible)
        {
            dataPanel.GetView().Hide();
            return;
        }

        dataPanel.plantName = plantName;
        dataPanel.plantType = plantType;

        dataPanel.SetData();
    }

    // FB panel management
    public void SetFlowerBedDataPanel(FlowerBed flowerBedRef)
    {
        TextMeshProUGUI[] texts = flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        TMP_Dropdown type = flowerBedDataPanel.GetComponentInChildren<TMP_Dropdown>();

        SpawnFlowerBedMenu(flowerBedRef);
        flowerBedMenuScript.SetFlowerBedHandler(flowerBedRef);
        if (!flowerBedDataPanel.IsVisible)
            flowerBedDataPanel.Show();
        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt.name == "Name")
                txt.text = flowerBedRef.flowerBedName;
        }
        if (flowerBedRef.groundType != "PLACEHOLDER")
        {
            foreach (TMP_Dropdown.OptionData data in type.options)
            {
                if (data.text == flowerBedRef.groundType)
                    type.value = type.options.IndexOf(data);
            }
        }
        flowerBed = flowerBedRef;
    }

    public void UpdateNameFlowerBed(string updateName)
    {
        TextMeshProUGUI[] texts = flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt.name == "Name")
                txt.text = updateName;
        }

        flowerBed.flowerBedName = updateName;
    }

    public void UpdateTypeFlowerBed(string updateType)
    {
        flowerBed.groundType = updateType;
    }

    // utils
    public bool GridButtonIsTrigger()
    {
        LabelScript tmp = gridButton.GetComponentInChildren<LabelScript>();
        if (tmp != null)
            return tmp.pressed;
        return false;
    }

    public void StartNewFb()
    {
        uIInteractions.StartNewFB();
    }

    public MenuScript GetMenuScript() { return menu; }

    public MenuFlowerBedScript GetFlowerBedMenuScript() { return flowerBedMenuScript; }

    public void SetCurrentHideViews(UIView view) { currentHideViews.Add(view); }

    public List<UIView> GetCurrentHideView() { return currentHideViews; }

    public FlowerBed GetFlowerBed() { return flowerBed; }

    public GhostHandler GetGhost() { return ghost; }
}
