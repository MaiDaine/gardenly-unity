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

    protected Transform previewUI = null;
    protected MenuScript menu = null;
    protected MenuFlowerBedScript flowerBedMenuScript = null;
    protected bool subMenuOpen = true;

    public void Cancel()
    {
        //TODO CLEAN
        if (menuOpen)
            this.menu.DestroyMenu();
        //Camera.main.GetComponent<UIController>().GetMenuScript().DestroyMenu();
        if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();
        //Camera.main.GetComponent<UIController>().GetFlowerBedMenuScript().DestroyMenu();
    }

    private void SpawnMenu(GhostHandler selectable, Transform menuType, bool isFlowerBed = false)
    {
        Canvas canvas;
        Vector3 position;

        if (menuOpen)
            this.menu.DestroyMenu();
        else if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();

        if (!isFlowerBed)
            position = new Vector3(selectable.transform.position.x, selectable.transform.position.y + 3, selectable.transform.position.z);
        else
        {
            FlowerBed tmp = (FlowerBed)selectable;
            
            position = new Vector3(tmp.GetVertices()[0].x, selectable.transform.position.y + 3, selectable.transform.position.z + 3);
        }

        this.previewUI = Instantiate(menuType, position, Quaternion.identity);
        canvas = this.previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        if (!isFlowerBed)
        {
            this.menu = this.previewUI.GetComponent<MenuScript>();
            menuOpen = true;
        }
        else
        {
            this.flowerBedMenuScript = this.previewUI.GetComponent<MenuFlowerBedScript>();
            flowerBedMenuOpen = true;
        }
    }


    public Transform GetPreviewUI()
    {
        return this.previewUI;
    }

    public MenuScript GetMenuScript()
    {
        return this.menu;
    }

    public MenuFlowerBedScript GetFlowerBedMenuScript()
    {
        return this.flowerBedMenuScript;
    }

    public void SpawnDynMenu(GhostHandler ghost, Transform typeMenu)
    {
        if (this.menu != null && this.menu.rotateState)
          return;

        SpawnMenu(ghost, typeMenu);
        this.menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBed flowerBed)
    {
        //FlowerBedHandler handler = mesh.GetOwner();

        if (this.flowerBedMenuScript != null)
            return;

        SpawnMenu(flowerBed, this.flowerBedMenu, true);
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

        labels[0].text = tmp.objectName;
        labels[1].text = tmp.description;

        // close your eyes
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

        button[1].onClick.AddListener(delegate { ConstructionController.instance.SpawnGhost(handler); });

        this.dataPanel.gameObject.SetActive(true);
    }
}