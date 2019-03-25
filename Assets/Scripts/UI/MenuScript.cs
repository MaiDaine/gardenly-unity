using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public bool rotateState = false;
    public bool isMoving = false;

    private GhostHandler ghost;
    private ConstructionController constructionController;

    void Start()
    {
        constructionController = ConstructionController.instance;
    }

    void LateUpdate()
    {
        if (ghost != null && !rotateState)
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
        this.isMoving = false;
        UIController.menuOpen = false;
    }

    public void DestroyGhost(GhostHandler ghost)
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
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

    public void MoveGhost()
    {
      LabelScript tmpScript = this.GetComponentInChildren<LabelScript>();

      tmpScript.ResetColor();
      rotateState = false;
      isMoving = true;
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

        if (ghost.transform.localEulerAngles.x <= 270)
            ghost.transform.Rotate(Vector3.up, -rotx);
        else
            ghost.transform.Rotate(Vector3.forward, -rotx);
    }
}
