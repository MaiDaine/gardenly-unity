using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Transform dynamicObjectMenu;
    public Transform flowerBedMenu;
    public Transform wallMenu;
    public static bool menuOpen = false;

    protected Transform previewUI;
    protected MenuScript menu;
    // Start is called before the first frame update
    void Start()
    {
        previewUI = null;
        menu = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        
    }

    void SpawnMenu(GhostHandler selectable, Transform menuType, FlowerBedMesh mesh = null)
    {
        Canvas canvas;
        Vector3 position;

        if (mesh == null)
            position = new Vector3(selectable.transform.position.x, selectable.transform.position.y + 3, selectable.transform.position.z);
        else
            position = new Vector3(mesh.transform.position.x, mesh.transform.position.y + 3, mesh.transform.position.z);

        previewUI = Instantiate(menuType, position, Quaternion.identity);
        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        menu = previewUI.GetComponent<MenuScript>();
        menuOpen = true;
    }

    public Transform GetPreviewUI()
    {
        return previewUI;
    }

    public MenuScript GetMenuScript()
    {
        return menu;
    }

    public void SpawnDynMenu(GhostHandler ghost, Transform typeMenu)
    {
        SpawnMenu(ghost, typeMenu);
        menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu(FlowerBedMesh mesh)
    {
        FlowerBedHandler handler = FlowerBedHandler.instance;

        SpawnMenu(handler, flowerBedMenu, mesh);
        menu.SetFlowerBedHandler(handler);
    }

    public void SpawnWallMenu(GhostHandler ghost)
    {
        SpawnMenu(ghost, wallMenu);
        menu.SetGhostRef(ghost);
    }
}
