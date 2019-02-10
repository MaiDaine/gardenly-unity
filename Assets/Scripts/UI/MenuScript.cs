using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public bool rotateState = false;

    private GhostHandler ghost;
    private Camera player;
    private ConstructionController constructionController;
    private FlowerBedHandler flowerBedHandler;

    // Start is called before the first frame update
    void Start()
    {
        constructionController = ConstructionController.instance;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ghost != null && !rotateState && flowerBedHandler == null)
            this.transform.position = new Vector3(ghost.transform.position.x, ghost.transform.position.y + 3, ghost.transform.position.z);
    }

    public void SetGhostRef(GhostHandler ghostRef)
    {
        ghost = ghostRef;
    }

    public GhostHandler GetGhost()
    {
        return ghost;
    }

    public void DestroyMenu()
    {
        Destroy(this.gameObject);
        this.rotateState = false;
        UIController.menuOpen = false;
    }

    public void DestroyGhost(GhostHandler ghost)
    {
        if (constructionController.StateIsOff())
        {
            Destroy(ghost.gameObject);
            Destroy(ghost);
            DestroyMenu();
        }
    }

    public void DestroyDynObj()
    {
        DestroyGhost(this.ghost);
    }

    public void DestroyFlowerBedHandler()
    {
        DestroyGhost(this.flowerBedHandler);
    }

    public void MoveGhost()
    {
        if (!rotateState)
            constructionController.SetGhost(ghost);
    }

    public void StartRotate()
    {
        rotateState = !rotateState;
    }

    public void RotateGhost()
    {
        float rotSpeed = 100f;
        float rotx = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        if (ghost.transform.eulerAngles.x == 0)
            ghost.transform.Rotate(Vector3.up, -rotx);
        else
            ghost.transform.Rotate(Vector3.forward, -rotx);
    }

    public void SetFlowerBedHandler(FlowerBedHandler handler)
    {
        flowerBedHandler = handler;
    }

    public void register()
    {
        flowerBedHandler.CombineMesh();
    }

    public void addFlowerBedMesh()
    {
        flowerBedHandler.SpawnMesh();
        constructionController.SetConstructionState(ConstructionController.ConstructionState.Building);
    }
}
