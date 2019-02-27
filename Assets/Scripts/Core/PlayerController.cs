using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;
    public List<ISelectable> currentSelection = new List<ISelectable>();
    public const int layerMaskInteractible = (1 << 9);
    public const int layerMaskStatic = (1 << 10);

    private Plane groundPlane = new Plane(Vector3.forward, Vector3.up);
    private IInteractible interactible;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    void Update()
    {
        Vector3 pos;
        RaycastHit hit;

        //DEBUG
        if (ConstructionController.instance.MouseRayCast(out pos, out hit))
            Debug.DrawLine(Camera.main.transform.position, pos);

        if (Input.GetKeyDown(KeyCode.L))
            ReactProxy.instance.ExportScene();


        if (ConstructionController.instance.GetConstructionState() == ConstructionController.ConstructionState.Off)
         {
             if (Input.GetMouseButtonDown(0))
                 SelectBuilding();
             else if (Input.GetKey(KeyCode.Delete))
                 DestroySelection();
         }

         if (ConstructionController.instance.GetConstructionState() == ConstructionController.ConstructionState.Editing)
         {
             if (Input.GetMouseButtonDown(0))
             {
                if (ConstructionController.instance.MouseRayCast(out pos, out hit, layerMaskInteractible))
                    interactible = hit.collider.gameObject.GetComponent<IInteractible>();
                else if (ConstructionController.instance.MouseRayCast(out pos, out hit, layerMaskStatic))
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
                ConstructionController.instance.UpdateEditing(interactible);
             if (interactible != null && Input.GetMouseButtonUp(0))
             {
                 interactible.EndDrag();
                 interactible = null;
             }
         }
        else if (ConstructionController.instance.GetConstructionState() != ConstructionController.ConstructionState.Off)
            ConstructionController.instance.UpdateGhost();
    }

    void SelectBuilding()
    {
        Vector3 pos;
        RaycastHit hit;

        DeSelect();
        if (ConstructionController.instance.MouseRayCast(out pos, out hit, layerMaskStatic))
        {
            ISelectable selectable = hit.collider.gameObject.GetComponent<ISelectable>();

            if (selectable != null)
            {
                currentSelection.Add(selectable);
                if (Input.GetKey(KeyCode.LeftControl))
                    currentSelection.AddRange(selectable.SelectWithNeighbor());
                else
                    selectable.Select(ConstructionController.instance.GetConstructionState());
            }
        }
    }

    public void ForcedSelection(ISelectable elem)
    {
        currentSelection.Clear();
        currentSelection.Add(elem);
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
