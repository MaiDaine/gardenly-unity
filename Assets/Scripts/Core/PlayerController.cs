using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;
    public const int layerMaskInteractible = (1 << 9);
    public const int layerMaskStatic = (1 << 10);

    public GameObject canvas;
    public List<ISelectable> currentSelection = new List<ISelectable>();

    private Plane groundPlane = new Plane(Vector3.forward, Vector3.up);
    private IInteractible interactible;
    private ConstructionController constructionController;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        constructionController = ConstructionController.instance;
    }

    void Update()
    {
        //DEBUG
        Vector3 pos;
        RaycastHit hit;

        if (this.constructionController.MouseRayCast(out pos, out hit))
            Debug.DrawLine(Camera.main.transform.position, pos);
        if (Input.GetKeyDown(KeyCode.L))
            ReactProxy.instance.ExportScene();
        if (Input.GetKeyDown(KeyCode.P))
            SpawnController.instance.SpawnFlowerBed();
        //END DEBUG

        //Selection
        if (this.constructionController.currentState == ConstructionController.ConstructionState.Off)
         {
             if (Input.GetMouseButtonDown(0))
                 SelectBuilding();
             else if (Input.GetKey(KeyCode.Delete))
                 DestroySelection();
         }

        //Ghost Handle
         if (this.constructionController.currentState == ConstructionController.ConstructionState.Editing)
         {
             if (Input.GetMouseButtonDown(0))
             {
                if (this.constructionController.MouseRayCast(out pos, out hit, layerMaskInteractible))
                    this.interactible = hit.collider.gameObject.GetComponent<IInteractible>();
                else if (this.constructionController.MouseRayCast(out pos, out hit, layerMaskStatic))
                {
                    ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();
                    if (selectable != null)
                    {
                        selectable.Select(ConstructionController.ConstructionState.Editing);
                        currentSelection.Clear();
                    }
                }
                if (interactible == null)
                    DeSelect(true);
             }
             else if (Input.GetMouseButton(0) && interactible != null)
                this.constructionController.UpdateGhostEditing(interactible);
             if (interactible != null && Input.GetMouseButtonUp(0))
             {
                 interactible.EndDrag();
                 interactible = null;
             }
         }
        else if (this.constructionController.currentState != ConstructionController.ConstructionState.Off)
            this.constructionController.UpdateGhost();
    }


    //Selection Handle
    private void SelectBuilding()
    {
        Vector3 pos;
        RaycastHit hit;

        if (GraphicRaycast())
            return;

        DeSelect();
        if (constructionController.MouseRayCast(out pos, out hit, layerMaskStatic))
        {
            ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();

            if (selectable != null)
            {
                this.currentSelection.Add(selectable);
                if (Input.GetKey(KeyCode.LeftControl))
                    this.currentSelection.AddRange(selectable.SelectWithNeighbor());
                else
                    selectable.Select(constructionController.currentState);
            }
        }
    }

    public void SpawnFlowerBedMesh()
    {
        Camera.main.GetComponent<UIController>().Cancel();
        SpawnController.instance.SpawnFlowerBed();
    }

    public void ForcedSelection(ISelectable elem)
    {
        foreach (ISelectable item in this.currentSelection)
            item.DeSelect();
        this.currentSelection.Clear();
        this.currentSelection.Add(elem);
    }

    private void DeSelect(bool forced = false)
    {
        if (this.currentSelection.Count > 0 && (!Input.GetKey(KeyCode.LeftShift)) || forced)
        {
            foreach (ISelectable elem in this.currentSelection)
                elem.DeSelect();
            this.currentSelection.Clear();
        }
    }

    private void DestroySelection()
    {
        if (this.currentSelection != null)
        {
            for (int i = 0; i < this.currentSelection.Count; i++)
                GameObject.Destroy(this.currentSelection[i].GetGameObject());
            this.currentSelection.Clear();
        }
    }

    private bool GraphicRaycast()
    {
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        PointerEventData pointerEventData;
        EventSystem eventSystem = canvas.GetComponent<EventSystem>();

        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        return results.Count != 0;
    }
}
