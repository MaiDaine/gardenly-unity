using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum ConstructionState { Off, Positioning, Building};

    Camera viewCamera;
    public WallHandler Ghost;
    private ConstructionState currentState = ConstructionState.Off;

    void Start()
    {
        viewCamera = Camera.main;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (currentState == ConstructionState.Off && Input.GetKey("p"))
            SpawnGhost();

        if (currentState != ConstructionState.Off)
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane goundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (goundPlane.Raycast(ray, out rayDistance))
            {
                Debug.DrawLine(ray.origin, ray.GetPoint(rayDistance), Color.red);

                if (currentState == ConstructionState.Positioning)
                    UpdateGhostPosition(ray.GetPoint(rayDistance));
                if (currentState == ConstructionState.Positioning && Input.GetMouseButtonDown(0))
                    StartConstruction(ray.GetPoint(rayDistance));
            }
        }
    }

    void SpawnGhost()
    {
        Ghost = Instantiate(Ghost, Vector3.zero, Quaternion.identity) as WallHandler;  
        currentState = ConstructionState.Positioning;
    }

    void UpdateGhostPosition(Vector3 point)
    {
        Ghost.transform.position = point;
    }

    void StartConstruction(Vector3 point)
    {

    }
}
