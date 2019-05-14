using UnityEngine;

public class MenuScript : MonoBehaviour, IMenu
{
    public bool rotateState = false;
    public bool isMoving = false;
    public bool isHidden = false;

    private GhostHandler ghost;
    private ConstructionController constructionController;
    private PlayerController playerController;

    private void Start()
    {
        this.constructionController = ConstructionController.instance;
        this.playerController = PlayerController.instance;
    }

   /* private void LateUpdate()
    {
        Quaternion rotation;
        Vector3 relativePos;

        if (this.ghost != null && !this.rotateState)
            this.transform.position = new Vector3(this.ghost.transform.position.x, 
                this.ghost.transform.position.y + 3, this.ghost.transform.position.z);
        
        relativePos = this.transform.position - Camera.main.transform.position;
        rotation = Quaternion.LookRotation(relativePos,Vector3.up);
        this.transform.rotation = rotation;
        
    }*/


    public void SetGhostRef(GhostHandler ghostRef) { this.ghost = ghostRef; }

    public GhostHandler GetGhost() { return this.ghost; }

    public void DestroyMenu()
    {
        UIController controller = Camera.main.GetComponent<UIController>();
        //if (controller.dynamicObjectMenu.IsVisible)
            controller.dynamicObjectMenu.Hide();
        //if (controller.wallMenu.IsVisible)
            controller.wallMenu.Hide();
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
        this.playerController.actionHandler.NewEditonAction(ConstructionController.EditionType.Position, this.playerController.currentSelection);
        //this.constructionController.SetGhost(ghost);
    }

    public void StartRotate()
    {
        this.playerController.actionHandler.NewEditonAction(ConstructionController.EditionType.Rotation, this.playerController.currentSelection);
        this.rotateState = !this.rotateState;//TODO CHECK
    }

    public void EditionEnd()
    {
        Debug.Log("EDITION END");
        LabelScript[] tmpScripts = this.GetComponentsInChildren<LabelScript>();

        foreach (LabelScript labelScript in tmpScripts)
        {
            labelScript.ResetColor();
        }
        this.rotateState = false;
        this.isMoving = false;
    }

    public GameObject GetGameObject() { return this.gameObject; }

    public bool IsHidden() { return this.isHidden; }

    public void SetHidden(bool state) { this.isHidden = state; }

    public void DestroyObject()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
            PlayerController.instance.DestroySelection();
    }
}