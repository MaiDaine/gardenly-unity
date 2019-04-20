using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public enum ConstructionState { Off, Positioning, Building, Editing };
    public enum EditionType { Off, Position, Rotation };
    public static ConstructionController instance = null;

    public ConstructionState currentState = ConstructionState.Off;
    public EditionType editionState = EditionType.Off;
    public float snapDistance = 0.15f;
    public int flowerbedCount = 0;

    private Camera Camera;
    private GridController Grid;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private GhostHandler ghost = null;
    private Vector3 lastPos = new Vector3(0, 0, 0);
    private Vector3 lastCast = new Vector3(0, 0, 0);
    private bool gridtate = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    private void Start()
    {
        this.Camera = Camera.main;
        this.Grid = GetComponent<GridController>();
    }


    public void ChangeGridState() { this.gridtate = !this.gridtate; }

    //MouseRayCast
    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, int layer = PlayerController.layerMaskStatic)
    {
        Ray ray = this.Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (this.groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer) && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            this.lastCast = pos;
            return true;
        }
        pos = this.lastCast;
        hit = new RaycastHit();
        return false;
    }

    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, out ISnapable snapable, int layer = PlayerController.layerMaskStatic)
    {
        Ray ray = this.Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (this.groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer, QueryTriggerInteraction.Ignore)
           && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            this.lastCast = pos;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            return true;
        }
        pos = this.lastCast;
        hit = new RaycastHit();
        snapable = null;
        return false;
    }


    //Ghost Handling functions
    public void Cancel()
    {
        if (ghost != null)
        {
            Destroy(ghost.gameObject);
            ghost.OnCancel();
            ghost = null;
            currentState = ConstructionState.Off;
        }
    }

    public void SpawnGhost(GhostHandler GhostRef)
    {
        this.Camera.GetComponent<UIController>().Cancel();
        if (currentState != ConstructionState.Off)
            Cancel();
        this.ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity);
        this.currentState = ConstructionState.Positioning;
        if (this.gridtate)
            this.Grid.activ = true;
    }

    public void SetGhost(GhostHandler ghost)
    {
        this.ghost = ghost;
        //this.currentState = ConstructionState.Positioning;
        if (this.gridtate)
            this.Grid.activ = true;
    }

    public void EditPositioning(GhostHandler GhostRef)
    {
        this.ghost = GhostRef;
        this.currentState = ConstructionState.Positioning;
        if (this.gridtate)
            this.Grid.activ = true;
    }

    public void UpdateGhost()
    {
        Vector3 tmp;
        RaycastHit hit;
        ISelectable neighbor = null;
        ISnapable snapable = null;
        
        if (MouseRayCast(out tmp, out hit))
        {
            if (tmp == this.lastPos && !Input.GetMouseButtonDown(0))
                return;
            this.lastPos = tmp;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            if ((snapable != null) && (snapable.FindSnapPoint(ref tmp, this.snapDistance)))
            {
                this.lastPos = tmp;
                if (snapable.isLinkable())
                    neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
            }
            else if (((tmp = Grid.GetNearestPointOnGrid(lastPos)) - this.lastPos).magnitude < this.snapDistance)
                this.lastPos = tmp;
            if (this.currentState == ConstructionState.Positioning)
                UpdateGhostPositioning(this.lastPos, neighbor);
            else
                UpdateGhostBuilding(this.lastPos, neighbor);
        }
    }

    private void UpdateGhostPositioning(Vector3 pos, ISelectable neighbor)
    {
        this.ghost.Positioning(pos);
        if (Input.GetMouseButtonDown(0))
        {
            if (this.ghost.needFlowerBed)
            {
                Vector3 currentPos;
                RaycastHit hit;

                MouseRayCast(out currentPos, out hit);
                if (hit.collider.gameObject.tag != "FlowerBed")
                    ErrorHandler.instance.ErrorMessage("Must be placed in a Flowerbed");
                else
                {
                    hit.collider.GetComponent<FlowerBed>().AddElement((FlowerBedElement)ghost);
                    this.currentState = ConstructionState.Off;
                    if (this.gridtate)
                        this.Grid.activ = false;
                    this.ghost.EndConstruction(pos);
                }
                return;
            }
            AddNeighbor(neighbor);
            if (this.ghost.FromPositioningToBuilding(pos))
                this.currentState = ConstructionState.Building;
        }
    }

    public void UpdateGhostBuilding(Vector3 pos, ISelectable neighbor)
    {
        if (this.ghost.Building(pos) || Input.GetMouseButtonDown(0))
        {
            AddNeighbor(neighbor);
            this.currentState = ConstructionState.Off;
            this.ghost.EndConstruction(pos);
            this.Grid.activ = false;
            UIController uIController = Camera.main.GetComponent<UIController>();
            if (uIController.GetMenuScript() != null)
                uIController.GetMenuScript().isMoving = false;
        }
    }

    public void UpdateGhostEditing(IInteractible interactible)
    {
        Vector3 pos;
        Vector3 tmp;
        RaycastHit hit;
        ISnapable snap;

        if (MouseRayCast(out pos, out hit, out snap))
        {
            if (snap != null)
                snap.FindSnapPoint(ref pos, this.snapDistance);
            else if (((tmp = Grid.GetNearestPointOnGrid(pos)) - pos).magnitude < this.snapDistance)
                pos = tmp;
            interactible.DragClick(pos);
        }
    }

    public void EditPosition(Vector3 position)//TODO TMP
    {
        this.ghost.Move(position);
    }

    //ISelectable Helper
    private void AddNeighbor(ISelectable neighbor)
    {
        if (neighbor != null)
        {
            this.ghost.AddNeighbor(neighbor);
            neighbor.AddNeighbor(this.ghost);
        }
    }
}
