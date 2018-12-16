using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum ConstructionState { Off, Positioning, Building};

    Camera viewCamera;
    public WallHandler Ghost;
    private ConstructionState currentState = ConstructionState.Off;
    private const int layerMask = 1 << 9;
    private List<ISelectable> currentSelection = new List<ISelectable>();

    void Start()
    {
        viewCamera = Camera.main;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (currentState == ConstructionState.Off)
        {
            if (Input.GetKey(KeyCode.P))
                SpawnGhost();
            else if (Input.GetMouseButtonDown(0))
                SelectBuilding();
        }

        if (currentState != ConstructionState.Off)
            UpdateGhost();
    }

    void SpawnGhost()
    {
        Ghost = Instantiate(Ghost, Vector3.zero, Quaternion.identity) as WallHandler;  
        currentState = ConstructionState.Positioning;
    }

    void UpdateGhost()
    {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance) 
            && Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (currentState == ConstructionState.Positioning)
            {
                Ghost.transform.position = ray.GetPoint(rayDistance);
                if (Input.GetMouseButtonDown(0))
                    StartConstruction(ray.GetPoint(rayDistance));
            }
            else
            {
                Ghost.Preview(ray.GetPoint(rayDistance));
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

    void SelectBuilding()
    {
        if (currentSelection.Count > 0 && !Input.GetKey(KeyCode.LeftShift))
        {
            foreach (ISelectable elem in currentSelection)
                elem.DeSelect();
            currentSelection.Clear();
        }

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float rayDistance = (viewCamera.transform.position - Vector3.zero).magnitude;
        if (Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            ISelectable[] selectables = hit.collider.gameObject.GetComponents<ISelectable>();
            for (int i = 0; i < selectables.Length; i++)
            {
                selectables[i].Select();
                currentSelection.Add(selectables[i]);
            }
        }

    }
}