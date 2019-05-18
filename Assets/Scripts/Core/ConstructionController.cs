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
    public RaycastHit lastCastHit;
    public List<FlowerBed> flowerBeds = new List<FlowerBed>();

    private Camera Camera;
    private GridController Grid;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private GhostHandler ghost = null;
    private Vector3 lastValidPos = new Vector3(0, 0, 0);
    private Vector3 lastCastPos = new Vector3(0, 0, 0);
    private bool gridState = true;

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


    public void ChangeGridState() { this.gridState = !this.gridState; }

    //MouseRayCast
    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, int layer = PlayerController.layerMaskStatic)
    {
        Ray ray = this.Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (this.groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer) && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            this.lastCastPos = pos;
            return true;
        }
        pos = this.lastCastPos;
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
            this.lastCastPos = pos;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            return true;
        }
        pos = this.lastCastPos;
        hit = new RaycastHit();
        snapable = null;
        return false;
    }


    //Ghost Handling functions
    public void Cancel()
    {
        if (this.currentState == ConstructionState.Off || this.currentState == ConstructionState.Editing)
        {
            PlayerController.instance.DeSelect(true);
            this.ghost = null;
            return;
        }

        if (this.ghost != null)
        {
            if (this.currentState != ConstructionState.Off)
            {
                if (this.ghost.OnCancel())
                    Destroy(ghost.gameObject);
                else
                    ghost.gameObject.SetActive(false);
            }
            this.ghost = null;
            this.Grid.activ = false;
            this.currentState = ConstructionState.Off;
        }
    }

    public void SpawnGhost(GhostHandler GhostRef)
    {
        if (this.currentState == ConstructionState.Off)
        {
            this.Camera.GetComponent<UIController>().Cancel(true);
            Cancel();
            this.currentState = ConstructionState.Positioning;
            this.ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity);
            this.ghost.SetData(GhostRef.GetData());
            if (this.gridState)
                this.Grid.activ = true;
        }
    }

    public void SetGhost(GhostHandler ghost)
    {
        this.ghost = ghost;
        //this.currentState = ConstructionState.Positioning;
        if (this.gridState)
            this.Grid.activ = true;
    }

    public void EditPositioning(GhostHandler GhostRef)
    {
        this.ghost = GhostRef;
        this.currentState = ConstructionState.Positioning;
        if (this.gridState)
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
            lastCastHit = hit;
            if (tmp == this.lastValidPos && !Input.GetMouseButtonDown(0))
                return;
            this.lastValidPos = tmp;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            if ((snapable != null) && (snapable.FindSnapPoint(ref tmp, this.snapDistance)))
            {
                this.lastValidPos = tmp;
                if (snapable.isLinkable())
                    neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
            }
            else if (((tmp = Grid.GetNearestPointOnGrid(lastValidPos)) - this.lastValidPos).magnitude < this.snapDistance)
                this.lastValidPos = tmp;
            if (this.currentState == ConstructionState.Positioning)
                UpdateGhostPositioning(this.lastValidPos, neighbor);
            else
                UpdateGhostBuilding(this.lastValidPos, neighbor);
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
                    MessageHandler.instance.ErrorMessage("flower_bed", "invalid_pos");
                else
                {
                    hit.collider.GetComponent<FlowerBed>().AddElement((FlowerBedElement)ghost);
                    this.currentState = ConstructionState.Off;
                    if (this.gridState)
                        this.Grid.activ = false;
                    this.ghost.EndConstruction(pos);
                    PlayerController.instance.actionHandler.NewStateAction("Create", ghost.gameObject);
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
            if (ghost.GetComponent<ShapeCreator>() != null)
                PlayerController.instance.OnFlowerBedSpawn();
            else
                PlayerController.instance.actionHandler.NewStateAction("Create", ghost.gameObject);
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

    public void EditPosition(Vector3 position)
    {
        this.ghost.Move(position);
    }

    public bool CompleteEditPosition(Vector3 position)
    {
        if (this.ghost.needFlowerBed)
        {
            Vector3 currentPos;
            RaycastHit hit;

            MouseRayCast(out currentPos, out hit);
            if (hit.collider.gameObject.tag != "FlowerBed")
            {
                MessageHandler.instance.ErrorMessage("flower_bed", "invalid_pos");
                return false;
            }
        }

        this.ghost.Move(position);
        return true;
    }

    public void EditRotation(float input)
    {
        this.ghost.Rotate(input);
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
