using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public enum ConstructionState { Off, Positioning, Building, Editing };

    public WallHandler WallHandlerRef;//need one of each for serialization
    public DefaultStaticElement[] staticElements = new DefaultStaticElement[4];
    public static ConstructionController instance = null;

    private Camera Camera;
    private GridController Grid;
    private const int layerMaskInteractible = (1 << 9);
    private const int layerMask = 1 << 10;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    private float snapDistance = 0.15f;

    private ConstructionState currentState = ConstructionState.Off;
    private GhostHandler Ghost;
    private Vector3 lastPos = new Vector3(0, 0, 0);
    private Vector3 lastCast = new Vector3(0, 0, 0);

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

    public void EditPositioning(GhostHandler GhostRef)
    {
        Ghost = GhostRef;
        currentState = ConstructionState.Positioning;
        Grid.activ = true;
    }

    public void SpawnGhost(GhostHandler GhostRef)
    {
        Ghost = Instantiate(GhostRef, Vector3.zero, Quaternion.identity);
        currentState = ConstructionState.Positioning;
        Grid.activ = true;
    }
    
    public bool MouseRayCast(out Vector3 pos, out RaycastHit hit, int layer = layerMask)
    {
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)
           && Physics.Raycast(ray, out hit, rayDistance, layer, QueryTriggerInteraction.Ignore))
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
           && Physics.Raycast(ray, out hit, rayDistance, layer, QueryTriggerInteraction.Ignore))
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
                neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
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
            AddNeighbor(neighbor);
            currentState = ConstructionState.Building;
            Ghost.StartPreview(pos);
        }
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

    public void SpawnScene(SerializationData[] data)
    {
        WallHandler wallHandler;
        DefaultStaticElement staticElement;
        DefaultStaticElement.SerializableItem subType;

        for (int i = 0; i < data.Length; i++)
        {
            switch (data[i].type)
            {
                case SerializationController.ItemType.WallHandler:
                    wallHandler = Instantiate(WallHandlerRef, Vector3.zero, Quaternion.identity);
                    wallHandler.DeSerialize(data[i].serializedData);
                    break;

                case SerializationController.ItemType.DefaultStaticElement:
                    subType = JsonUtility.FromJson<DefaultStaticElement.SerializableItem>(data[i].serializedData);
                    switch (subType.subType)
                    {
                        case DefaultStaticElement.StaticElementType.Chair:
                            staticElement = Instantiate(staticElements[0], Vector3.zero, Quaternion.identity);
                            break;
                        case DefaultStaticElement.StaticElementType.Table:
                            staticElement = Instantiate(staticElements[1], Vector3.zero, Quaternion.identity);
                            break;
                        default:
                            //SHOULD NOT HAPPEN
                            staticElement = Instantiate(staticElements[0], Vector3.zero, Quaternion.identity);
                            break;
                       //TODO
                    }
                    staticElement.DeSerialize(data[i].serializedData);
                    break;
                default:
                    break;
            }
        }
    }
}
