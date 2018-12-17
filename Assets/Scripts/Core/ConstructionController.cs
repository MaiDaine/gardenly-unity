using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public enum ConstructionState { Off, Positioning, Building };

    private Camera Camera;
    private const int layerMask = 1 << 9;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private float snapDistance = 0.25f;

    private ConstructionState currentState = ConstructionState.Off;
    private WallHandler Ghost;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public ConstructionState GetConstructionState() { return currentState; }

    public void Init(Camera main, float inSnapDistance)
    {
        Camera = main;
        snapDistance = inSnapDistance;
    }

    public void SpawnGhost(WallHandler GhostRef)
    {
        Ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity) as WallHandler;
        currentState = ConstructionState.Positioning;
    }

    public void UpdateGhost()
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        RaycastHit hit;
        Vector3 bestPosition;

        if (groundPlane.Raycast(ray, out rayDistance)
            && Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            bestPosition = ray.GetPoint(rayDistance);
            ISnapable snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            if (snapable != null)
                bestPosition = snapable.FindSnapPoint(bestPosition, snapDistance);
            if (currentState == ConstructionState.Positioning)
            {
                Ghost.transform.position = bestPosition;
                if (Input.GetMouseButtonDown(0))
                    StartConstruction(bestPosition);
            }
            else
            {
                Ghost.Preview(bestPosition);
                if (Input.GetMouseButtonDown(0))
                    EndConstruction();
            }
        }
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
    }
}
