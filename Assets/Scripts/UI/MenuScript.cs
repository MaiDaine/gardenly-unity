using UnityEngine;

public class MenuScript : MonoBehaviour, IMenu
{
    public bool rotateState = false;
    public bool isMoving = false;
    public bool isHidden = false;

    private GhostHandler ghost;
    private ConstructionController constructionController;

    private void Start()
    {
        this.constructionController = ConstructionController.instance;
    }

    private void LateUpdate()
    {
        Quaternion rotation;
        Vector3 relativePos;

        if (this.ghost != null && !this.rotateState)
            this.transform.position = new Vector3(this.ghost.transform.position.x, 
                this.ghost.transform.position.y + 3, this.ghost.transform.position.z);
        
        relativePos = this.transform.position - Camera.main.transform.position;
        rotation = Quaternion.LookRotation(relativePos,Vector3.up);
        this.transform.rotation = rotation;
        
    }


    public void SetGhostRef(GhostHandler ghostRef) { this.ghost = ghostRef; }

    public GhostHandler GetGhost() { return this.ghost; }

    public void DestroyMenu()
    {
        Destroy(this.gameObject);
        this.rotateState = false;
        this.isMoving = false;
        UIController.menuOpen = false;
    }

    public void MoveGhost()
    {
        LabelScript tmpScript = this.GetComponentInChildren<LabelScript>();

        tmpScript.ResetColor();
        this.rotateState = false;
        this.isMoving = true;
        PlayerController.instance.CreateAction(ConstructionController.EditionType.Position);
        //this.constructionController.SetGhost(ghost);
    }

    public void StartRotate() { this.rotateState = !this.rotateState; }

    public void RotateGhost()
    {
        float rotSpeed = 100f;
        float rotx = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;

        if (this.ghost.transform.localEulerAngles.x <= 270)
            this.ghost.transform.Rotate(Vector3.up, -rotx);
        else
            this.ghost.transform.Rotate(Vector3.forward, -rotx);
    }

    public GameObject GetGameObject() { return this.gameObject; }

    public bool IsHidden() { return this.isHidden; }

    public void SetHidden(bool state) { this.isHidden = state; }

    public void DestroyObject()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
        {
            Destroy(this.ghost.gameObject);
            DestroyMenu();
        }
    }
}
