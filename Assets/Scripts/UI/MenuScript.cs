using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public bool rotateState = false;

    private DefaultStaticElement ghost;
    private Camera player;
    private ConstructionController constructionController;

    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main;
        constructionController = player.GetComponent<ConstructionController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ghost != null && !rotateState)
            this.transform.position = new Vector3(ghost.transform.position.x, ghost.transform.position.y + 6, ghost.transform.position.z);
    }

    public void SetGhostRef(DefaultStaticElement ghostRef)
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
    }

    public void DestroyGhost()
    {
        if (constructionController.StateIsOff())
        {
            Destroy(this.ghost.gameObject);
            Destroy(this.ghost);
            Destroy(this.gameObject);
        }
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
}
