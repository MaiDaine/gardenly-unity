using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;
    public const int layerMaskInteractible = (1 << 9);
    public const int layerMaskStatic = (1 << 10);

    public List<ISelectable> selectionList = new List<ISelectable>();
    public ISelectable currentSelection = null;
    public ActionHandler actionHandler;

    private Plane groundPlane = new Plane(Vector3.forward, Vector3.up);
    private IInteractible interactible;
    private ConstructionController constructionController;
    private CameraController cameraController;
    private Raycaster raycaster;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        constructionController = ConstructionController.instance;
        actionHandler.Initialize();
        cameraController = Camera.main.GetComponent<CameraController>();
        raycaster = Camera.main.GetComponent<Raycaster>();
        if (Application.isEditor)
            LocalisationController.instance.Init("FR");
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            constructionController.Cancel();
            Camera.main.GetComponentInChildren<UIController>().Cancel();
            Camera.main.GetComponentInChildren<UIController>().ResetButton(true);
        }

        //Selection
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            if (PlaneClick())
                SelectBuilding();
            else if (Input.GetKey(KeyCode.Delete))
                DestroySelection();
            return;
        }

        //Ghost Handle
        if (constructionController.currentState == ConstructionController.ConstructionState.Editing)
        {
            switch (constructionController.editionState)
            {
                case ConstructionController.EditionType.Off:
                {
                    if (PlaneClick())
                        SelectBuilding();
                    else if (Input.GetMouseButton(0) && interactible != null)
                        constructionController.UpdateGhostEditing(interactible);
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
                     if (raycaster.MouseRayCast(out pos, out hit))
                         constructionController.EditPosition(pos);
                     break;
                 }
                case ConstructionController.EditionType.Rotation:
                 {
                     constructionController.EditRotation(Input.GetAxis("Mouse X"));
                     break;
                 }
            }
            if (PlaneClick() && constructionController.editionState != ConstructionController.EditionType.Off)
            {
                if (constructionController.editionState == ConstructionController.EditionType.Position)
                {
                    Vector3 pos;
                    RaycastHit hit;
                    if (!(raycaster.MouseRayCast(out pos, out hit) && constructionController.CompleteEditPosition(pos)))
                        return;
                }
                actionHandler.ActionComplete(true);
            } 
        }
        else
            constructionController.UpdateGhost();

        if (!cameraController.inputEnabled)
            return;

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
    }

    //Selection Handle
    public void SelectFromAction(ISelectable selectable)
    {
        DeSelect();
        if (constructionController.currentState == ConstructionController.ConstructionState.Editing)
            constructionController.currentState = ConstructionController.ConstructionState.Off;
        selectionList.Add(selectable);
        currentSelection = selectable;
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
        if (selectionList != null)
        {
            for (int i = 0; i < selectionList.Count; i++)
                actionHandler.NewStateAction("Destroy", selectionList[i].GetGameObject());
            selectionList.Clear();
            Camera.main.GetComponent<UIController>().Cancel();
        }
    }

    public void DeSelect(bool forced = false)
    {
        if (selectionList.Count > 0 && (!Input.GetKey(KeyCode.LeftShift)) || forced)
        {
            foreach (ISelectable elem in selectionList)
                elem.DeSelect();
            selectionList.Clear();
        }
        if (currentSelection != null)
        {
            currentSelection.DeSelect();
            currentSelection = null;
        }
    }

    private void SelectBuilding()
    {
        Vector3 pos;
        RaycastHit hit;

        if (IsPointerOnUi())
            return;

        DeSelect();
        if (raycaster.MouseRayCast(out pos, out hit, layerMaskStatic))
        {
            ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();

            if (selectable != null)
            {
                if (constructionController.currentState == ConstructionController.ConstructionState.Editing)
                    constructionController.currentState = ConstructionController.ConstructionState.Off;
                selectionList.Add(selectable);
                if (Input.GetKey(KeyCode.LeftControl))
                    selectionList.AddRange(selectable.SelectWithNeighbor());
                else
                {
                    currentSelection = selectable;
                    selectable.Select(constructionController.currentState);
                }
            }
        }
    }

    //Tools
    public void OnFlowerBedSpawn()//TODO Special FB_Create to revert in edition mode
    {
        actionHandler.NewStateAction("Create", currentSelection.GetGameObject());
        actionHandler.revertActionSet.ClearTmpAction();
    }

    public bool PlaneClick()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOnUi())
            return true;
        return false;
    }

    private bool IsPointerOnUi() { return (EventSystem.current.IsPointerOverGameObject()); }
}
