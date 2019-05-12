using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class UIController : MonoBehaviour
{
    public UIView dynamicObjectMenu;
    public UIView wallMenu;
    public UIView dataPanel;
    public UIView flowerBedDataPanel;
    public UIView tutoView;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;
    public FlowerBedPanelScript flowerBedPanelScript;

    protected Transform previewUI = null;
    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected bool subMenuOpen = true;
    protected GhostHandler ghost = null;
    protected FlowerBed flowerBed = null;

   /* private void LateUpdate()
    {
        if (this.menu != null && !this.menu.rotateState && !this.menu.isMoving)
            DisplayMenu(this.menu);
        if (flowerBedMenuScript != null)
            DisplayMenu(this.flowerBedMenuScript);
    }

    private void DisplayMenu(IMenu menu)
    {
        if (Mathf.Abs(menu.GetGameObject().transform.position.x - Camera.main.transform.position.x) > 40
           || Mathf.Abs(menu.GetGameObject().transform.position.y - Camera.main.transform.position.y) > 20
           || Mathf.Abs(menu.GetGameObject().transform.position.z - Camera.main.transform.position.z) > 40)
            menu.GetGameObject().SetActive(false);
        else
            menu.GetGameObject().SetActive(true);
    }*/

    private void SpawnMenu(GhostHandler selectable, UIView menuType)
    {
        if (menuOpen)
            this.menu.DestroyMenu();
        menuType.Show();
        this.menu = menuType.GetComponent<MenuScript>();
        menuOpen = true;
    }

    private void SpawnFlowerBedMenu(FlowerBed flowerBed, UIView menuType)
    {
        if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();
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

    public void Cancel()
    {
       /* if (menuOpen)
            this.menu.DestroyMenu();
        if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();*/
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

    public void SpawnWallMenu(GhostHandler ghost)
    {
        if (this.menu != null && this.menu.rotateState)
            return;

        SpawnMenu(ghost, this.wallMenu);
        this.menu.SetGhostRef(ghost);
    }

    public void SetDataPanel(string plantName, string plantType)
    {
        PlantData tmp = ReactProxy.instance.externalData.plants[plantType][plantName];
        TextMeshProUGUI[] labels = this.dataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();
        RawImage icon = this.dataPanel.GetComponentInChildren<RawImage>();
        UIButton[] button = this.dataPanel.GetComponentsInChildren<UIButton>();
        ButtonScript script = button[0].GetComponent<ButtonScript>();

        if (tmp == null)
            return;

        if (labels != null && labels.Length > 0)
        {
            labels[0].text = tmp.name;
            labels[1].text = tmp.description;
            labels[11].text = tmp.phRangeLow + ", " + tmp.phRangeHigh;
        }
        // TMP
        if (sliders != null && sliders.Length > 0)
        {
            sliders[0].value = tmp.waterNeed;
            sliders[1].value = tmp.sunNeed;
            sliders[2].value = tmp.rusticity;
        }
        if (icon != null)
            icon.texture = tmp.image;
        if (!this.dataPanel.IsVisible)
        {
            this.dataPanel.Show();
        }
    }

    public void SetFlowerBedDataPanel(FlowerBed flowerBed)
    {
        TextMeshProUGUI[] texts = this.flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();

        SpawnFlowerBedMenu(flowerBed, this.flowerBedDataPanel);
        this.flowerBedMenuScript.SetFlowerBedHandler(flowerBed);
        if (!this.flowerBedDataPanel.IsVisible)
             this.flowerBedDataPanel.Toggle();
        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt.name == "Type")
                txt.text = flowerBed.soilType;
            if (txt.name == "Name")
                txt.text = flowerBed.name;
        }        
        this.flowerBed = flowerBed;
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
        
        this.flowerBed.name = updateName;
    }

    public void UpdateTypeFlowerBed(string updateType)
    {
        TextMeshProUGUI[] texts = this.flowerBedDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt.name == "Type")
                txt.text = updateType;
        }
        this.flowerBed.soilType = updateType;
    }
}