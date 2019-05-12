using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class UIController : MonoBehaviour
{
    public Transform gardenMenu;
    public Transform dynamicObjectMenu;
    public Transform flowerBedMenu;
    public Transform wallMenu;
    public UIView dataPanel;
    public Transform flowerBedDataPanel;
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

    private void LateUpdate()
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
    }

    private void SpawnMenu(GhostHandler selectable, Transform menuType)
    {
        Canvas canvas;
        Vector3 position;

        if (menuOpen)
            this.menu.DestroyMenu();

        position = new Vector3(selectable.transform.position.x, selectable.transform.position.y + 3, selectable.transform.position.z);
        this.previewUI = Instantiate(menuType, position, Quaternion.identity);
        canvas = this.previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;       
        this.menu = this.previewUI.GetComponent<MenuScript>();
        menuOpen = true;
    }

    private void SpawnFlowerBedMenu(FlowerBed flowerBed, Transform menuType)
    {
        Canvas canvas;
        Vector3 position;

        if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();

        // TODO Vector3.x = middle of flowerbed
        position = new Vector3(flowerBed.GetVertices()[0].x, flowerBed.transform.position.y + 3, flowerBed.transform.position.z + 3);

        this.previewUI = Instantiate(menuType, position, Quaternion.identity);
        canvas = this.previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        this.flowerBedMenuScript = this.previewUI.GetComponent<MenuFlowerBedScript>();
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
        if (menuOpen)
            this.menu.DestroyMenu();
        if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();
    }

    public Transform GetPreviewUI() { return this.previewUI; }

    public MenuScript GetMenuScript() { return this.menu; }

    public MenuFlowerBedScript GetFlowerBedMenuScript() { return this.flowerBedMenuScript; }

    public void SpawnDynMenu(GhostHandler ghost, Transform typeMenu)
    {
        if (this.menu != null && this.menu.rotateState)
          return;

        SpawnMenu(ghost, typeMenu);
        this.menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBed flowerBed)
    {
        SpawnFlowerBedMenu(flowerBed, this.flowerBedMenu);
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
        Image[] icons = this.dataPanel.GetComponentsInChildren<Image>();
        UIButton[] button = this.dataPanel.GetComponentsInChildren<UIButton>();
        ButtonScript script = button[0].GetComponent<ButtonScript>();

        if (tmp == null)
            return;

        
        Debug.Log(tmp.name);
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

        // icons[4].color = Color.green;
        if (!this.dataPanel.IsVisible)
        {
            this.dataPanel.Toggle();
       
        }
    }

    public void SetFlowerBedDataPanel(FlowerBed flowerBed)
    {
        Text[] texts = this.flowerBedDataPanel.GetComponentsInChildren<Text>();

        this.gardenMenu.gameObject.SetActive(true);
        this.flowerBedDataPanel.gameObject.SetActive(true);
        texts[1].text = flowerBed.soilType;
        texts[0].text = flowerBed.name;
        this.flowerBed = flowerBed;
    }

    public void ResetFlowerBedDataPanel()
    {
        this.flowerBed.name = "PLACEHOLDER";
    }

    public void UpdateFlowerBedDataPanel(string updateName)
    {
        Text[] texts = this.flowerBedDataPanel.GetComponentsInChildren<Text>();

        texts[0].text = updateName;
        this.flowerBed.name = updateName;
    }
}