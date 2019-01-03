using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public bool rotateState = false;

    private DynamicElemHandler ghost;
    private Camera player;
    private ConstructionController constructionController;

    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main;
        constructionController = player.GetComponent<ConstructionController>();
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void SetGhostRef(DynamicElemHandler ghostRef)
    {
        ghost = ghostRef;
    }

    public DynamicElemHandler GetGhost()
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
        
        ghost.transform.Rotate(Vector3.up, -rotx);
    }
}
