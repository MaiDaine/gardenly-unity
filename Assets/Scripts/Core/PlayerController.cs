using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public WallHandler GhostRef;
    public float snapDistance = 0.25f;

    private Camera Camera;
    private ConstructionController Construct;
    private const int layerMask = 1 << 9;
    private List<ISelectable> currentSelection = new List<ISelectable>();

    void Start()
    {
        Construct = GetComponent<ConstructionController>();
        Construct.Init(Camera.main, snapDistance);
        Camera = Camera.main;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (Construct.GetConstructionState() == ConstructionController.ConstructionState.Off)
        {
            if (Input.GetKeyDown(KeyCode.P))
                Construct.SpawnGhost(GhostRef);
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