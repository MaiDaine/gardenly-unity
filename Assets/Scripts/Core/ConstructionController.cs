using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public enum ConstructionState { Off, Positioning, Building, Editing };

    public static ConstructionController instance = null;

    private Camera Camera;
    private GridController Grid;
    private const int layerMaskInteractible = (1 << 9);
    private const int layerMask = 1 << 10;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private float snapDistance = 0.15f;
    private ConstructionState currentState = ConstructionState.Off;
    private GhostHandler Ghost = null;
    private Vector3 lastPos = new Vector3(0, 0, 0);
    private Vector3 lastCast = new Vector3(0, 0, 0);
    public int flowerbedCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public ConstructionState GetConstructionState() { return currentState; }
    public void SetConstructionState(ConstructionState state)
    {
        currentState = state;
    }

    public void Init(Camera main, float inSnapDistance, GridController grid)
    {
        Camera = main;
        snapDistance = inSnapDistance;
        Grid = grid;
    }

    public void Cancel()
    {
        if (Grid.activ || Ghost != null)
        {
            Destroy(Ghost.gameObject);
            Ghost = null;
            currentState = ConstructionState.Off;
            Grid.activ = false;
        }
        //else
        //TODO : interface => show user menu(options, sound ...)
    }

    public void EditPositioning(GhostHandler GhostRef)
    {
        Ghost = GhostRef;
        currentState = ConstructionState.Positioning;
        Grid.activ = true;
    }

    public void SpawnGhost(GhostHandler GhostRef)
    {
        if (currentState != ConstructionState.Off)
            Cancel();
        //TODO TEST;
        Ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity);
        currentState = ConstructionState.Positioning;
        Grid.activ = true;
    }

    public void SetGhost(DefaultStaticElement ghost)
    {
        if (currentState == ConstructionState.Off)
        {
            Ghost = ghost;
            currentState = ConstructionState.Positioning;
            Grid.activ = true;
        }
    }
    
    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, int layer = layerMask)
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer) && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            lastCast = pos;
            return true;
        }
        pos = lastCast;
        hit = new RaycastHit();
        return false;
    }

    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, out ISnapable snapable, int layer = layerMask)
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
       
        if (groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer, QueryTriggerInteraction.Ignore)
           && hit.collider.tag != "Invalid")
        {
            pos = ray.GetPoint(rayDistance);
            lastCast = pos;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            return true;
        }
        pos = lastCast;
        hit = new RaycastHit();
        snapable = null;
        return false;
    }

    public void UpdateGhost()
    {
        Vector3 tmp;
        RaycastHit hit;
        ISelectable neighbor = null;
        ISnapable snapable;
        if (MouseRayCast(out tmp, out hit))
        {
            if (tmp == lastPos && !Input.GetMouseButtonDown(0))
                return;
            lastPos = tmp;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            if ((snapable != null) && (snapable.FindSnapPoint(ref tmp, snapDistance)) && (snapable.isLinkable()))
            {
                lastPos = tmp;
                neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
            }
            else if (((tmp = Grid.GetNearestPointOnGrid(lastPos)) - lastPos).magnitude < snapDistance)
                lastPos = tmp;
            if (currentState == ConstructionState.Positioning)
                UpdateGhostPositioning(lastPos, neighbor);
            else
                UpdateGhostBuilding(lastPos, neighbor);
        }
    }

    public void UpdateEditing(IInteractible interactible)
    {
        Vector3 pos;
        Vector3 tmp;
        RaycastHit hit;
        ISnapable snap;
        if (MouseRayCast(out pos, out hit, out snap))
        {
            if (snap != null)
                snap.FindSnapPoint(ref pos, snapDistance);
            else if (((tmp = Grid.GetNearestPointOnGrid(pos)) - pos).magnitude < snapDistance)
                pos = tmp;
           interactible.DragClick(pos);
        }
    }

    private void UpdateGhostPositioning(Vector3 pos, ISelectable neighbor)
    {
        Ghost.transform.position = pos;
        if (Input.GetMouseButtonDown(0))
        {
            if (Ghost.needFlowerBed)
            {
                Vector3 currentPos;
                RaycastHit hit;
                MouseRayCast(out currentPos, out hit);
                if (hit.collider.gameObject.tag != "FlowerBed")
                    ErrorHandler.instance.ErrorMessage("Must be placed in a Flowerbed");
                else
                {
                    hit.collider.GetComponent<FlowerBedHandler>().AddElement((FlowerBedElement)Ghost);
                    currentState = ConstructionState.Off;
                    Grid.activ = false;
                    Ghost.EndPreview();
                }
                return;
            }
            AddNeighbor(neighbor);
            currentState = ConstructionState.Building;
            Ghost.StartPreview(pos);
        }
    }

    public bool StateIsOff()
    {
        if (currentState == ConstructionState.Off)
            return true;
        return false;
    }

    public void UpdateGhostBuilding(Vector3 pos, ISelectable neighbor)
    {
        Ghost.Preview(pos);
        if (Input.GetMouseButtonDown(0))
        {
            AddNeighbor(neighbor);
            currentState = ConstructionState.Off;
            Ghost.EndPreview();
            Grid.activ = false;
        }
    }

    private void AddNeighbor(ISelectable neighbor)
    {
        if (neighbor != null)
        {
            Ghost.AddNeighbor(neighbor);
            neighbor.AddNeighbor(Ghost);
        }
    }
}
