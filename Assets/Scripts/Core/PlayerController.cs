using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float snapDistance = 0.25f;
    public Plane groundPlane = new Plane(Vector3.forward, Vector3.up);
    public FlowerBedHandler bed;

    private Camera Camera;
    private ConstructionController Construct;
    private GridController Grid;
    private const int layerMask = 1 << 9;
    private List<ISelectable> currentSelection = new List<ISelectable>();
    
    void Start()
    {
        Camera = Camera.main;
        Grid = GetComponent<GridController>();                
        Construct = GetComponent<ConstructionController>();
        Construct.Init(Camera.main, snapDistance, Grid);
    }

    void LateUpdate()
    {
        if (Construct.GetConstructionState() == ConstructionController.ConstructionState.Off)
        {
            Vector3 pos;
            RaycastHit hit;
            if (Construct.MouseRayCast(out pos, out hit))
                Debug.DrawLine(Camera.transform.position, pos);

            if (Input.GetKeyDown(KeyCode.O))
                Grid.activ = !Grid.activ;
            if (Input.GetKeyDown(KeyCode.P))
                bed.AddPoint(new Vector2(pos.x, pos.z));
            if (Input.GetKeyDown(KeyCode.M))
                bed.Init();
            else if (Input.GetMouseButtonDown(0))
                SelectBuilding();
            else if (Input.GetKey(KeyCode.Delete))
                DestroySelection();
        }

        if (Construct.GetConstructionState() != ConstructionController.ConstructionState.Off)
            Construct.UpdateGhost();
    }

    void SelectBuilding()
    {
        if (currentSelection.Count > 0 && !Input.GetKey(KeyCode.LeftShift))
        {
            foreach (ISelectable elem in currentSelection)
                elem.DeSelect();
            currentSelection.Clear();
        }

        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float rayDistance = (Camera.transform.position - Vector3.zero).magnitude;
        if (Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();
            if (selectable != null)
            {
                currentSelection.Add(selectable);
                if (Input.GetKey(KeyCode.LeftControl))
                    currentSelection.AddRange(selectable.SelectWithNeighbor());
                else
                    selectable.Select();
            }
        }
    }
        
    void DestroySelection()
    {
        if (currentSelection != null)
        {
            for (int i = 0; i < currentSelection.Count; i++)
                GameObject.Destroy(currentSelection[i].GetGameObject());
            currentSelection.Clear();
        }
    }
}