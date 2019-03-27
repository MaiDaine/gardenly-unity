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

    private void Start()
    {
        this.constructionController = ConstructionController.instance;
    }

    private void LateUpdate()
    {
        if (this.ghost != null && !this.rotateState)
            this.transform.position = new Vector3(this.ghost.transform.position.x, 
                this.ghost.transform.position.y + 3, this.ghost.transform.position.z);
    }


    public void SetGhostRef(GhostHandler ghostRef)
    {
        this.ghost = ghostRef;
    }

    public GhostHandler GetGhost()
    {
        return this.ghost;
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
        this.rotateState = false;
        this.isMoving = true;
        this.constructionController.SetGhost(ghost);
    }

    public void StartRotate()
    {
        this.rotateState = !this.rotateState;
    }

    public void RotateGhost()
    {
        float rotSpeed = 100f;
        float rotx = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;

        if (this.ghost.transform.localEulerAngles.x <= 270)
            this.ghost.transform.Rotate(Vector3.up, -rotx);
        else
            this.ghost.transform.Rotate(Vector3.forward, -rotx);
    }
}
