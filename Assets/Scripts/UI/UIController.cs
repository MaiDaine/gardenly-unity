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
        ObjectsData tmp = handler.GetData();
        Text[] labels = dataPanel.GetComponentsInChildren<Text>();
        Slider[] sliders = dataPanel.GetComponentsInChildren<Slider>();
        Image[] icons = dataPanel.GetComponentsInChildren<Image>();
        Button[] button;

        labels[0].text = tmp.objectName;
        labels[1].text = tmp.description;
        sliders[0].value = tmp.water;
        sliders[1].value = tmp.sunshine;
        sliders[2].value = tmp.solidity;
        icons[4].color = Color.green;

        button = dataPanel.GetComponentsInChildren<Button>();
        button[1].onClick.AddListener(delegate { ConstructionController.instance.SpawnGhost(handler); });
        dataPanel.gameObject.SetActive(true);
    }
}