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
    public List<ISelectable> selectionList = new List<ISelectable>();
    public ISelectable currentSelection = null;
    public ActionHandler actionHandler;

    private Plane groundPlane = new Plane(Vector3.forward, Vector3.up);
    private IInteractible interactible;
    private ConstructionController constructionController;
    private CameraController cameraController;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        this.constructionController = ConstructionController.instance;
        this.actionHandler.Initialize();
        this.cameraController = Camera.main.GetComponent<CameraController>();
        if (Application.isEditor)
            LocalisationController.instance.Init("FR");
    }

    private void Update()
    {
        if (!cameraController.inputEnabled)
            return;

        if (Input.GetKey(KeyCode.Escape))
        {
            this.constructionController.Cancel();
            Camera.main.GetComponentInChildren<UIController>().Cancel();
        }
        
        //Redo - Revert
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            GhostAction currentAction = actionHandler.RedoAction();
            if (currentAction != null)
                UpdateSelectionAfterAction(currentAction);
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            GhostAction currentAction = actionHandler.RevertAction();
            if (currentAction != null)
                UpdateSelectionAfterAction(currentAction);
        }

        //Selection
        if (this.constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            if (Input.GetMouseButtonDown(0))
                SelectBuilding();
            else if (Input.GetKey(KeyCode.Delete))
                DestroySelection();
            return;
        }

        //Ghost Handle
        if (this.constructionController.currentState == ConstructionController.ConstructionState.Editing)
        {
            switch (constructionController.editionState)
            {
                case ConstructionController.EditionType.Off:
                {
                    if (Input.GetMouseButtonDown(0))
                        SelectBuilding();
                    else if (Input.GetMouseButton(0) && interactible != null)
                        this.constructionController.UpdateGhostEditing(interactible);
                    if (interactible != null && Input.GetMouseButtonUp(0))
                    {
                        interactible.EndDrag();
                        interactible = null;
                    }
                    break;
                }
                case ConstructionController.EditionType.Position:
                 {
                     Vector3 pos;
                     RaycastHit hit;
                     if (constructionController.MouseRayCast(out pos, out hit))
                         constructionController.EditPosition(pos);
                     break;
                 }
                case ConstructionController.EditionType.Rotation:
                 {
                     constructionController.EditRotation(Input.GetAxis("Mouse X"));
                     break;
                 }
            }
            if (Input.GetMouseButtonDown(0) && constructionController.editionState != ConstructionController.EditionType.Off)
                actionHandler.ActionComplete(true);
        }
        else
            this.constructionController.UpdateGhost();
    }

    //Selection Handle
    private void SelectBuilding()
    {
        Vector3 pos;
        RaycastHit hit;

        if (IsPointerOnUi())
            return;

        DeSelect();
        if (constructionController.MouseRayCast(out pos, out hit, layerMaskStatic))
        {
            ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();

            if (selectable != null)
            {
                if (this.constructionController.currentState == ConstructionController.ConstructionState.Editing)
                    this.constructionController.currentState = ConstructionController.ConstructionState.Off;
                this.selectionList.Add(selectable);
                if (Input.GetKey(KeyCode.LeftControl))
                    this.selectionList.AddRange(selectable.SelectWithNeighbor());
                else
                {
                    this.currentSelection = selectable;
                    selectable.Select(constructionController.currentState);
                }
            }
        }
    }

    public void SelectFromAction(ISelectable selectable)
    {
        DeSelect();
        if (this.constructionController.currentState == ConstructionController.ConstructionState.Editing)
            this.constructionController.currentState = ConstructionController.ConstructionState.Off;
        this.selectionList.Add(selectable);
        this.currentSelection = selectable;
        selectable.Select(constructionController.currentState);
    }

    public void UpdateSelectionAfterAction(GhostAction action)
    {
        if (currentSelection != null)
            currentSelection.DeSelect();
        currentSelection = action.GetGameObject().GetComponent<ISelectable>();
        if (currentSelection != null)
            currentSelection.Select(ConstructionController.ConstructionState.Off);
    }

    public void DestroySelection()
    {
        if (this.selectionList != null)
        {
            for (int i = 0; i < this.selectionList.Count; i++)
                this.actionHandler.NewStateAction("Destroy", this.selectionList[i].GetGameObject());
            this.selectionList.Clear();
            UIController uIController = Camera.main.GetComponentInChildren<UIController>();
            if (uIController.GetMenuScript() != null)
                uIController.GetMenuScript().DestroyMenu();
            if (uIController.GetFlowerBedMenuScript() != null)
                uIController.GetFlowerBedMenuScript().DestroyMenu();
        }
    }

    public void DeSelect(bool forced = false)
    {
        if (this.selectionList.Count > 0 && (!Input.GetKey(KeyCode.LeftShift)) || forced)
        {
            foreach (ISelectable elem in this.selectionList)
                elem.DeSelect();
            this.selectionList.Clear();
        }
        if (currentSelection != null)
        {
            this.currentSelection.DeSelect();
            this.currentSelection = null;
        }
    }

    //Tools
    public void OnFlowerBedSpawn()//TODO Special FB_Create to revert in edition mode
    {
        this.actionHandler.NewStateAction("Create", currentSelection.GetGameObject());
        this.actionHandler.revertActionSet.ClearTmpAction();
    }

    public void SpawnFlowerBedMesh()
    {
        Camera.main.GetComponent<UIController>().Cancel();
        constructionController.currentState = ConstructionController.ConstructionState.Positioning;
        SpawnController.instance.SpawnFlowerBed();
    }

    private bool IsPointerOnUi() { return (EventSystem.current.IsPointerOverGameObject()); }
}
