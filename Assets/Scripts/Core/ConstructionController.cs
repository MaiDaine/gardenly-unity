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
    private WallHandler Ghost;
    private Vector3 lastRayCast = new Vector3(0, 0, 0);
    private bool lClick = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (currentState != ConstructionState.Off && Input.GetMouseButtonDown(0))
            lClick = true;
    }

    public ConstructionState GetConstructionState() { return currentState; }

    public void Init(Camera main, float inSnapDistance, GridController grid)
    {
        Camera = main;
        snapDistance = inSnapDistance;
        Grid = grid;
    }

    public void SpawnGhost(WallHandler GhostRef)
    {
        Ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity) as WallHandler;
        currentState = ConstructionState.Positioning;
        Grid.activ = true;
    }

    public void UpdateGhost()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        RaycastHit hit;
        Vector3 bestPosition;
        ISelectable neighbor = null;

        if (groundPlane.Raycast(ray, out rayDistance)
            && Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            bestPosition = ray.GetPoint(rayDistance);
            if (bestPosition == lastRayCast)
                return;
            lastRayCast = bestPosition;
            Vector3 tmp;
            ISnapable snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            if ((snapable != null) && (snapable.FindSnapPoint(ref bestPosition, snapDistance)) && (snapable.isLinkable()))
                neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
            else if (((tmp = Grid.GetNearestPointOnGrid(bestPosition)) - bestPosition).magnitude < snapDistance)
                bestPosition = tmp;
            if (currentState == ConstructionState.Positioning)
            {
                Ghost.transform.position = bestPosition;
                if (lClick)
                {
                    StartConstruction(bestPosition);
                    if (neighbor != null)
                    {
                        Ghost.AddNeighbor(neighbor);
                        neighbor.AddNeighbor(Ghost);
                    }
                }
            }
            else
            {
                Ghost.Preview(bestPosition);
                if (lClick)
                {
                    EndConstruction();
                    if (neighbor != null)
                    {
                        Ghost.AddNeighbor(neighbor);
                        neighbor.AddNeighbor(Ghost);
                    }
                }
            }
        }
        lClick = false;
    }

    void StartConstruction(Vector3 point)
    {
        currentState = ConstructionState.Building;
        Ghost.StartPreview(point);
    }

    void EndConstruction()
    {
        currentState = ConstructionState.Off;
        Ghost.EndPreview();
        Grid.activ = false;
    }
}
