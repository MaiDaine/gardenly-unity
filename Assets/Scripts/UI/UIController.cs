using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Doozy.Engine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    public UIView shadowMap;
    public UIView dynamicObjectMenu;
    public UIView extendMenu;
    public UIView flowerBedDataPanel;
    public UIView[] plantsViews;
    public UIButton gridButton;
    public UIButton cameraModeButton;
    public UIButton[] tmpBtn;
    public UIButtonListener uIButtonListener;
    public UIButtonListener mainButtonListener;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;
    public TextMeshProUGUI gardenName;
    public TMP_InputField hour;
    public TextMeshProUGUI displayHour;
    public PlantPanelScript dataPanel;
    public UIInteractions uIInteractions = null;
    public TutorialController tutorialController;
    public IEnumerator imageCoroutine = null;
    public GameEvent tutoBlock;
    public CompassScript compass;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;
    public static bool afterBuilding = false;

    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected GhostHandler ghost = null;
    protected FlowerBed flowerBed = null;
    protected List<UIView> currentHideViews = new List<UIView>();

    private ReactProxy reactProxy;
    private Vector3 anchorOpenView = new Vector3();
    private Vector3 anchorCloseView = new Vector3();
    private float sizeView = 183.48f;
    private bool compassToggle = false;

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
        for (int i = 0; i < 6; i++)
        {
            if (plantsViews[i].IsVisible)
                return true;
        }
        return false;
    }

    public void HideViews()
    {
        if (plantsViews != null)
        {
            foreach (UIView view in plantsViews)
            {
                if (view != null && view.gameObject != null && view.IsVisible)
                    view.Hide();
            }
            if (shadowMap.IsVisible)
                shadowMap.Hide();
        }
    }

    public void DestroyMenu()
    {
        if (dynamicObjectMenu != null && dynamicObjectMenu.IsVisible)
            dynamicObjectMenu.Hide();
        if (uIButtonListener != null)
            uIButtonListener.GetComponentInChildren<ViewController>().ResetButtons();
        if (mainButtonListener != null)
            mainButtonListener.GetComponentInChildren<ViewController>().ResetButtons();
        HideViews();
        if (dataPanel != null && dataPanel.GetView() != null)
            dataPanel.GetView().Hide();
        if (flowerBedDataPanel != null && flowerBedDataPanel.IsVisible)
            flowerBedDataPanel.Hide();
        menuOpen = false;
        flowerBedMenuOpen = false;
        if (ShadowMap.instance.startShadowCalc == 1)
            ShadowMap.instance.startShadowCalc = 0;
        if (compass != null && compass.gameObject.activeSelf)
        {
            compass.ClearInterface();
            compassToggle = false;
            compass.gameObject.SetActive(false);
        }
    }

    public void ToggleCompassInterface()
    {
        compassToggle = !compassToggle;
        compass.gameObject.SetActive(compassToggle);
        compass.ToggleInterface();
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
        for (int i = 0; i < 6; i++)
        {
            if (plantsViews[i].IsVisible)
                currentHideViews.Add(plantsViews[i]);
        }
        if (dataPanel.GetView().IsVisible)
            currentHideViews.Add(dataPanel.GetView());
    }

    // Plant data panel management
    public void SetDataPanel(string plantName, string plantType, bool onSelect = false)
    {
        anchorOpenView = new Vector3(-extendMenu.RectTransform.sizeDelta.x - sizeView + 0.3f, -55f, 0);
        anchorCloseView = new Vector3(-extendMenu.RectTransform.sizeDelta.x + 0.3f, -55f, 0);
        if (PlantsViewsDisplay() || afterBuilding)
        {
            dataPanel.GetView().CustomStartAnchoredPosition = anchorOpenView;
            afterBuilding = false;
        }
        else
            dataPanel.GetView().CustomStartAnchoredPosition = anchorCloseView;
        if (dataPanel.GetPlantDataRef() != null && dataPanel.GetPlantDataRef().name == plantName && dataPanel.GetView().IsVisible)
        {
            dataPanel.GetView().Hide();
            return;
        }
        dataPanel.SetData(plantType, plantName, onSelect);
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
        ShadowMap.instance.startShadowCalc = 0;
        uIInteractions.StartNewFB();
    }

    public void SendHours(DayNightController controller)
    {
        float hourVal = 0;
        if (float.TryParse(hour.text, out hourVal) && (hour.text.Length == 1 || hour.text.Length == 2)  && hourVal >= 0 && hourVal < 24)
        {
            controller.InstantSetTimeofDay(hourVal);
            displayHour.text = hour.text + " : 00";
            hour.text = "00";
        }
        else
            MessageHandler.instance.ErrorMessage("hour_format");
    }

    /*public void SendMinutes(DayNightController controller)
    {
        controller.InstantSetTimeofDay(minutes.text);
    }*/

    public MenuScript GetMenuScript() { return menu; }

    public void OnStartCoroutine(PlantData plantDataRef)
    {
        imageCoroutine = reactProxy.externalData.GetTexture(plantDataRef, plantDataRef.imgUrl);
        StartCoroutine(imageCoroutine);
    }

    public MenuFlowerBedScript GetFlowerBedMenuScript() { return flowerBedMenuScript; }

    public void SetCurrentHideViews(UIView view) { currentHideViews.Add(view); }

    public List<UIView> GetCurrentHideView() { return currentHideViews; }

    public FlowerBed GetFlowerBed() { return flowerBed; }

    public GhostHandler GetGhost() { return ghost; }
}
