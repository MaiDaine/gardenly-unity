using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Transform dynamicObjectMenu;
    public Transform flowerBedMenu;
    public Transform wallMenu;
    public Transform dataPanel;
    public static bool menuOpen = false;
    public static bool flowerBedMenuOpen = false;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;

    protected Transform previewUI = null;
    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected bool subMenuOpen = true;
    protected GhostHandler ghost = null;

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

    public void SetDataPanel(GhostHandler handler)
    {
        ObjectsData tmp = handler.GetData();
        Text[] labels = this.dataPanel.GetComponentsInChildren<Text>();
        Slider[] sliders = this.dataPanel.GetComponentsInChildren<Slider>();
        Image[] icons = this.dataPanel.GetComponentsInChildren<Image>();
        Button[] button = this.dataPanel.GetComponentsInChildren<Button>();
        ButtonScript script = button[1].GetComponent<ButtonScript>();

        this.dataPanel.gameObject.SetActive(true);

        script.SetGhost(handler);

        labels[0].text = tmp.objectName;
        labels[1].text = tmp.description;

        // TMP

        if (sliders[0].value == -1)
            sliders[0].gameObject.SetActive(false);
        else
        {
            sliders[0].gameObject.SetActive(true);
            sliders[0].value = tmp.water;
        }
        if (sliders[1].value == -1)
            sliders[1].gameObject.SetActive(false);
        else
        {
            sliders[1].gameObject.SetActive(true);
            sliders[1].value = tmp.sunshine;
        }
        if (sliders[2].value == -1)
            sliders[2].gameObject.SetActive(false);
        else
        {
            sliders[2].value = tmp.solidity;
            sliders[2].gameObject.SetActive(true);
        }
        icons[4].color = Color.green;
        
    }
}