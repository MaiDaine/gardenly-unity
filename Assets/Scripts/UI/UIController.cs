using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Transform dynamicObjectMenu;
    public Transform flowerBedMenu;
    public Transform wallMenu;

    protected Transform previewUI;
    protected MenuScript menu;
    // Start is called before the first frame update
    void Start()
    {
        previewUI = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        
    }

    void SpawnMenu(GhostHandler selectable, Transform menuType)
    {
        Canvas canvas;
        Vector3 position;

        position = new Vector3(selectable.transform.position.x, selectable.transform.position.y + 3, selectable.transform.position.z);

        previewUI = Instantiate(menuType, position, Quaternion.identity);
        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        menu = previewUI.GetComponent<MenuScript>();
    }

    public Transform GetPreviewUI()
    {
        return previewUI;
    }

    public MenuScript GetMenuScript()
    {
        return menu;
    }

    public void SpawnDynMenu(GhostHandler ghost)
    {
        //Canvas canvas;
        //Vector3 position;

        SpawnMenu(ghost, dynamicObjectMenu);
        /*position = new Vector3(ghost.transform.position.x, ghost.transform.position.y + 3, ghost.transform.position.z);

        previewUI = Instantiate(dynamicObjectMenu, position, Quaternion.identity);
        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        menu = previewUI.GetComponent<MenuScript>();*/
        menu.SetGhostRef(ghost);
    }

    public void SpawnFlowerBedMenu()
    {
        //Canvas canvas;
        FlowerBedHandler handler = FlowerBedHandler.instance;
        //Vector3 position;


        SpawnMenu(handler, flowerBedMenu);

        /*position = new Vector3(handler.transform.position.x, handler.transform.position.y + 3, handler.transform.position.z);

        previewUI = Instantiate(flowerBedMenu, position, Quaternion.identity);

        canvas = previewUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        menu = previewUI.GetComponent<MenuScript>();*/
        menu.SetFlowerBedHandler(handler);
    }
}
