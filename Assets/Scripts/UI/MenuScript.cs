using UnityEngine;
using Doozy.Engine.UI;

// Manage action of objects panel other than FB
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


    public void DestroyMenu()
    {
        this.rotateState = false;
        this.isMoving = false;
    }

    public void MoveGhost()
    {
        LabelScript tmpScript = this.GetComponentInChildren<LabelScript>();

        tmpScript.ResetColor();
        this.rotateState = false;
        this.isMoving = true;
        this.playerController.actionHandler.NewEditonAction(ConstructionController.EditionType.Position, this.playerController.currentSelection);
    }

    public void StartRotate()
    {
        this.playerController.actionHandler.NewEditonAction(ConstructionController.EditionType.Rotation, this.playerController.currentSelection);
        this.rotateState = !this.rotateState;
    }

    public void EditionEnd()
    {
        LabelScript[] tmpScripts = this.GetComponentsInChildren<LabelScript>();

        foreach (LabelScript labelScript in tmpScripts)
        {
            labelScript.ResetColor();
        }
        this.rotateState = false;
        this.isMoving = false;
        GridController.instance.activ = false;
    }

    public void DestroyObject()
    {
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
            PlayerController.instance.DestroySelection();
    }

    public void SetGhostRef(GhostHandler ghostRef) { this.ghost = ghostRef; }

    public GhostHandler GetGhost() { return this.ghost; }

    public GameObject GetGameObject() { return this.gameObject; }

    public bool IsHidden() { return this.isHidden; }

    public void SetHidden(bool state) { this.isHidden = state; }

}