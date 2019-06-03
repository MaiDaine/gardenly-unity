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
        constructionController = ConstructionController.instance;
        playerController = PlayerController.instance;
    }


    public void DestroyMenu()
    {
        GetComponentInChildren<LabelScript>().ResetColor();
        rotateState = false;
        isMoving = false;
    }

    public void MoveGhost()
    {
        LabelScript tmpScript = GetComponentInChildren<LabelScript>();

        tmpScript.ResetColor();
        rotateState = false;
        isMoving = true;
        playerController.actionHandler.NewEditonAction(ConstructionController.EditionType.Position, playerController.currentSelection);
    }

    public void StartRotate()
    {
        playerController.actionHandler.NewEditonAction(ConstructionController.EditionType.Rotation, playerController.currentSelection);
        rotateState = !rotateState;
    }

    public void EditionEnd()
    {
        LabelScript[] tmpScripts = GetComponentsInChildren<LabelScript>();

        foreach (LabelScript labelScript in tmpScripts)
        {
            labelScript.ResetColor();
        }
        rotateState = false;
        isMoving = false;

        // FIXME: Typo -> active
        // ENHANCEMENT: GridController selon moi devrait exposer des méthodes publique pour ce genre d'opération
        GridController.instance.activ = false;
    }

    public void DestroyObject()
    {
        // ENHANCEMENT: constructionController selon moi devrait exposer une méthodes publique pour ce genre d'opération
        if (constructionController.currentState == ConstructionController.ConstructionState.Off)
            PlayerController.instance.DestroySelection();
    }

    public void SetGhostRef(GhostHandler ghostRef) { ghost = ghostRef; }

    public GhostHandler GetGhost() { return ghost; }

    public GameObject GetGameObject() { return gameObject; }

    public bool IsHidden() { return isHidden; }

    public void SetHidden(bool state) { isHidden = state; }

}