using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public enum ConstructionState { Off, Positioning, Building };


    private Camera Camera;
    private GridController Grid;
    private const int layerMask = 1 << 9;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private float snapDistance = 0.25f;

    private ConstructionState currentState = ConstructionState.Off;
    private GhostHandler Ghost;
    private Vector3 lastPos = new Vector3(0, 0, 0);
    private Vector3 lastCast = new Vector3(0, 0, 0);

    void Start()
    {
        
    }

    void Update()
    {
    }

    public ConstructionState GetConstructionState() { return currentState; }

    public void Init(Camera main, float inSnapDistance, GridController grid)
    {
        Camera = main;
        snapDistance = inSnapDistance;
        Grid = grid;
    }

    public void SpawnGhost(GhostHandler GhostRef)
    {
        Ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity);
        currentState = ConstructionState.Positioning;
        Grid.activ = true;
    }
    
    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit)
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            pos = ray.GetPoint(rayDistance);
            lastCast = pos;
            return true;
        }
        pos = lastCast;
        hit = new RaycastHit();
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
                neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
            else if (((tmp = Grid.GetNearestPointOnGrid(lastPos)) - lastPos).magnitude < snapDistance)
                lastPos = tmp;
            if (currentState == ConstructionState.Positioning)
                UpdateGhostPositioning(lastPos, neighbor);
            else
                UpdateGhostBuilding(lastPos, neighbor);
        }
    }

    private void UpdateGhostPositioning(Vector3 pos, ISelectable neighbor)
    {
        Ghost.transform.position = pos;
        if (Input.GetMouseButtonDown(0))
        {
            AddNeighbor(neighbor);
            currentState = ConstructionState.Building;
            Ghost.StartPreview(pos);
        }
    }

    private void UpdateGhostBuilding(Vector3 pos, ISelectable neighbor)
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
