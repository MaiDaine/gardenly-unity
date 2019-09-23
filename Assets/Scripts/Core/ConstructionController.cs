using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public enum ConstructionState { Off, Positioning, Building, Editing };
    public enum EditionType { Off, Position, Rotation };
    public static ConstructionController instance = null;

    public ConstructionState currentState = ConstructionState.Off;
    public EditionType editionState = EditionType.Off;
    public List<FlowerBed> flowerBeds = new List<FlowerBed>();
    public float snapDistance = 0.15f;
    public RaycastHit lastCastHit;

    private Raycaster raycaster;
    private UIController uiController;
    private GhostHandler ghost = null;
    private Vector3 lastValidPos = new Vector3(0, 0, 0);
    private GridController grid;//TODO #73
    private bool gridState = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        grid = GetComponent<GridController>();
        uiController = Camera.main.GetComponent<UIController>();
        raycaster = Camera.main.GetComponent<Raycaster>();
    }

    public void UpdatePlantsSunExposure()
    {
        foreach (FlowerBed elem in flowerBeds)
            elem.UpdatePlantSunExposure();
    }

    //Ghost Handling functions
    public void Cancel()
    {
        uiController.SaveViews();
        uiController.Cancel();
        if (currentState == ConstructionState.Off || currentState == ConstructionState.Editing)
        {
            PlayerController.instance.DeSelect(true);
            ghost = null;
            return;
        }
        if (ghost != null)
        {
            if (currentState != ConstructionState.Off)
            {
                if (ghost.OnCancel())
                    Destroy(ghost.gameObject);
                else
                    ghost.gameObject.SetActive(false);
            }
            ghost = null;
            grid.activ = false;
            currentState = ConstructionState.Off;
        }
    }

    public GhostHandler GetGhost() { return ghost; }

    //TODO #74
    public void SpawnGhost(GhostHandler ghostRef)
    {
        TutoObject.waitForBuild = true;
        if (ghostRef.needFlowerBed && flowerBeds.Count < 1)
        {
            MessageHandler.instance.ErrorMessage("flower_bed", "no_flowerbed");
            return;
        }
        if (currentState == ConstructionState.Off || currentState == ConstructionState.Positioning)
        {
            if (currentState == ConstructionState.Positioning && ghost != null && ghost.name == ghostRef.name + "(Clone)")
            {
                Cancel();
                return;
            }
            Cancel();
            currentState = ConstructionState.Positioning;
            ghost = Instantiate(ghostRef, Vector3.zero, Quaternion.identity);
            ghost.SetData(ghostRef.GetData());
            if (gridState)//TODO #73
                grid.activ = true;
        }
    }

    //TODO #74
    public void LoadPlantGhost(GhostHandler ghost)
    {
        Cancel();
        currentState = ConstructionState.Positioning;
        this.ghost = ghost;
        if (gridState)//TODO #73
            grid.activ = true;
    }

    public void SetGhost(GhostHandler ghost)
    {
        TutoObject.waitForBuild = true;
        if (ghost != null && ghost.needFlowerBed && flowerBeds.Count < 1)
        {
            MessageHandler.instance.ErrorMessage("flower_bed", "no_flowerbed");
            return;
        }
        this.ghost = ghost;
        if (gridState)//TODO #73
            grid.activ = true;
    }

    //Ghost Update
    public void UpdateGhost()
    {
        Vector3 tmp;
        RaycastHit hit;
        ISelectable neighbor = null;
        ISnapable snapable = null;

        if (raycaster.MouseRayCast(out tmp, out hit))
        {
            lastCastHit = hit;
            if (tmp == lastValidPos && !PlayerController.instance.PlaneClick())
                return;
            lastValidPos = tmp;
            snapable = hit.collider.gameObject.GetComponent<ISnapable>();
            if ((snapable != null) && (snapable.FindSnapPoint(ref tmp, snapDistance)))
            {
                lastValidPos = tmp;
                if (snapable.isLinkable())
                    neighbor = snapable.GetGameObject().GetComponent<ISelectable>();
            }
            else if (((tmp = grid.GetNearestPointOnGrid(lastValidPos)) - lastValidPos).magnitude < snapDistance)
                lastValidPos = tmp;
            if (currentState == ConstructionState.Positioning)
                UpdateGhostPositioning(lastValidPos, neighbor);
            else
                UpdateGhostBuilding(lastValidPos, neighbor);
        }
    }

    private void UpdateGhostPositioning(Vector3 pos, ISelectable neighbor)
    {
        ghost.Positioning(pos);
        if (PlayerController.instance.PlaneClick())
        {
            if (ghost.needFlowerBed)
            {
                Vector3 currentPos;
                RaycastHit hit;

                raycaster.MouseRayCast(out currentPos, out hit);
                if (hit.collider.gameObject.tag != "FlowerBed")
                    MessageHandler.instance.ErrorMessage("flower_bed", "invalid_pos");
                else
                {
                    FlowerBed flowerBed = hit.collider.GetComponent<FlowerBed>();
                    flowerBed.AddElement((PlantElement)ghost);
                    ((PlantElement)ghost).SetTileKey(flowerBed.GetKey());
                    currentState = ConstructionState.Off;
                    if (gridState && !uiController.GridButtonIsTrigger())//TODO #73
                        grid.activ = false;
                    ghost.EndConstruction(pos);
                    PlayerController.instance.actionHandler.NewStateAction("Create", ghost.gameObject);
                }
                return;
            }
            AddNeighbor(neighbor);
            if (ghost.FromPositioningToBuilding(pos))
                currentState = ConstructionState.Building;
        }
    }

    public void UpdateGhostBuilding(Vector3 pos, ISelectable neighbor)
    {
        if (ghost.Building(pos) || PlayerController.instance.PlaneClick())
        {
            AddNeighbor(neighbor);
            currentState = ConstructionState.Off;
            ghost.EndConstruction(pos);
            if (ghost.GetComponent<ShapeCreator>() != null)
                PlayerController.instance.OnFlowerBedSpawn();
            else
                PlayerController.instance.actionHandler.NewStateAction("Create", ghost.gameObject);
            if (gridState && !uiController.GridButtonIsTrigger())//TODO #73
                grid.activ = false;
            if (uiController.GetMenuScript() != null)
                uiController.GetMenuScript().isMoving = false;
        }
    }

    public void UpdateGhostEditing(IInteractible interactible)
    {
        Vector3 pos;
        Vector3 tmp;
        RaycastHit hit;
        ISnapable snap;

        if (raycaster.MouseRayCast(out pos, out hit, out snap))
        {
            if (snap != null)
                snap.FindSnapPoint(ref pos, snapDistance);
            else if (((tmp = grid.GetNearestPointOnGrid(pos)) - pos).magnitude < snapDistance)
                pos = tmp;
            interactible.DragClick(pos);
        }
    }

    //UI Menu Links
    public void EditRotation(float input) { ghost.Rotate(input); }

    public void EditPosition(Vector3 position) { ghost.Move(position); }

    public bool CompleteEditPosition(Vector3 position)
    {
        if (ghost.needFlowerBed)
        {
            Vector3 currentPos;
            RaycastHit hit;
            //TODO Edit fb key
            raycaster.MouseRayCast(out currentPos, out hit);
            if (hit.collider.gameObject.tag != "FlowerBed")
            {
                MessageHandler.instance.ErrorMessage("flower_bed", "invalid_pos");
                return false;
            }
        }

        ghost.Move(position);
        return true;
    }

    //ISelectable Helper
    private void AddNeighbor(ISelectable neighbor)
    {
        if (neighbor != null)
        {
            ghost.AddNeighbor(neighbor);
            neighbor.AddNeighbor(ghost);
        }
    }
}
