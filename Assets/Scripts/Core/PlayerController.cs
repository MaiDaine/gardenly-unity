using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;
    public float snapDistance = 0.25f;
    public Plane groundPlane = new Plane(Vector3.forward, Vector3.up);

    private string serialization;
    private int numberItems;
    private Camera Camera;
    private ConstructionController Construct;
    private GridController Grid;
    private const int layerMaskInteractible = (1 << 9);
    private const int layerMaskStatic = (1 << 10);
    private IInteractible interactible;

    public List<ISelectable> currentSelection = new List<ISelectable>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    void Start()
    {
        Camera = Camera.main;
        Grid = GetComponent<GridController>();                
        Construct = GetComponent<ConstructionController>();
        Construct.Init(Camera.main, snapDistance, Grid);
    }

    public void ForcedSelection(ISelectable elem)
    {
        currentSelection.Clear();
        currentSelection.Add(elem);
    }

    void Update()
    {
        Vector3 pos;
        RaycastHit hit;

        //DEBUG
        if (Construct.MouseRayCast(out pos, out hit))
            Debug.DrawLine(Camera.transform.position, pos);

        if (Construct.GetConstructionState() == ConstructionController.ConstructionState.Off)
         {
             if (Input.GetMouseButtonDown(0))
                 SelectBuilding();
             else if (Input.GetKey(KeyCode.Delete))
                 DestroySelection();
         }

         if (Construct.GetConstructionState() == ConstructionController.ConstructionState.Editing)
         {
             if (Input.GetMouseButtonDown(0))
             {
                if (Construct.MouseRayCast(out pos, out hit, layerMaskInteractible))
                    interactible = hit.collider.gameObject.GetComponent<IInteractible>();
                else if (Construct.MouseRayCast(out pos, out hit, layerMaskStatic))
                {
                    ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();
                    if (selectable != null)
                    {
                        selectable.Select(ConstructionController.ConstructionState.Editing);
                        currentSelection.Clear();
                    }
                }
                if (interactible == null)
                    DeSelect(true);
             }
             else if (Input.GetMouseButton(0) && interactible != null)
                 Construct.UpdateEditing(interactible);
             if (interactible != null && Input.GetMouseButtonUp(0))
             {
                 interactible.EndDrag();
                 interactible = null;
             }
         }
        else if (Construct.GetConstructionState() != ConstructionController.ConstructionState.Off)
            Construct.UpdateGhost();
    }

    void DeSelect(bool forced = false)
    {
        if (currentSelection.Count > 0 && (!Input.GetKey(KeyCode.LeftShift)) || forced)
        {
            foreach (ISelectable elem in currentSelection)
                elem.DeSelect();
            currentSelection.Clear();
        }
    }

    void SelectBuilding()
    {
        Vector3 pos;
        RaycastHit hit;

        DeSelect();
        if (Construct.MouseRayCast(out pos, out hit, layerMaskStatic))
        {
            ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();
            if (selectable != null)
            {
                currentSelection.Add(selectable);
                if (Input.GetKey(KeyCode.LeftControl))
                    currentSelection.AddRange(selectable.SelectWithNeighbor());
                else
                    selectable.Select(Construct.GetConstructionState());
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