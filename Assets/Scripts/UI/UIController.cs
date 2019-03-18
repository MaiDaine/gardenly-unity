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

    void SpawnMenu(GhostHandler selectable, Transform menuType, FlowerBedMesh mesh = null)
    {
        Canvas canvas;
        Vector3 position;

        if (menuOpen)
            this.menu.DestroyMenu();
        else if (flowerBedMenuOpen)
            this.flowerBedMenuScript.DestroyMenu();
        if (mesh == null)
            position = new Vector3(selectable.transform.position.x, selectable.transform.position.y + 3, selectable.transform.position.z);
        else
            position = new Vector3(mesh.transform.position.x, mesh.transform.position.y + 3, mesh.transform.position.z);

        previewUI = Instantiate(menuType, position, Quaternion.identity);
        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        if (mesh == null)
        {
            menu = previewUI.GetComponent<MenuScript>();
            menuOpen = true;
        }
        else
        {
            flowerBedMenuScript = previewUI.GetComponent<MenuFlowerBedScript>();
            flowerBedMenuOpen = true;
        }
    }

    public Transform GetPreviewUI()
    {
        return previewUI;
    }

    public MenuScript GetMenuScript()
    {
        return menu;
    }

    public MenuFlowerBedScript GetFlowerBedMenuScript()
    {
        return flowerBedMenuScript;
    }

    public void SpawnDynMenu(GhostHandler ghost, Transform typeMenu)
    {
        if (menu != null && menu.rotateState)
          return;
        SpawnMenu(ghost, typeMenu);
        menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBedMesh mesh)
    {
        FlowerBedHandler handler = mesh.GetOwner();

        if (flowerBedMenuScript != null)
            return;
        SpawnMenu(handler, flowerBedMenu, mesh);
        flowerBedMenuScript.SetFlowerBedHandler(handler);
    }

    public void SpawnWallMenu(GhostHandler ghost)
    {
        if (menu != null && menu.rotateState)
            return;
        SpawnMenu(ghost, wallMenu);
        menu.SetGhostRef(ghost);
    }

    public void SetDataPanel(GhostHandler handler)
    {
        ObjectsData data = handler.GetData();
        Text[] labels = dataPanel.GetComponentsInChildren<Text>();
        Slider[] sliders = dataPanel.GetComponentsInChildren<Slider>();
       
        labels[0].text = data.objectName;
        labels[1].text = data.description;
        sliders[0].value = data.water;
        sliders[1].value = data.sunshine;
        sliders[2].value = data.solidity;
    }
}