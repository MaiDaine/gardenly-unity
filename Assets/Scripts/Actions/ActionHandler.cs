using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : ScriptableObject
{
    public Action currentAction = null;

    private ConstructionController constructionController;

    public void Initialize()
    {
        this.constructionController = ConstructionController.instance;
    }

    public void ActionComplete(ActionRuntimeSet revertActionSet)
    {
        this.currentAction.Complete();
        revertActionSet.Add(currentAction);
        this.currentAction = null;
        this.constructionController.editionState = ConstructionController.EditionType.Off;
        this.constructionController.currentState = ConstructionController.ConstructionState.Off;
    }

    public void EditPositioning(ISelectable selection)
    {
        this.currentAction = ScriptableObject.CreateInstance("Move") as Action;
        this.constructionController.currentState = ConstructionController.ConstructionState.Editing;
        this.constructionController.editionState = ConstructionController.EditionType.Position;

        this.currentAction.Initialize(selection.GetGameObject());
        this.constructionController.SetGhost(selection.GetGameObject().GetComponent<GhostHandler>());
    }

    public void EditRotation(ISelectable selection)
    {
        this.currentAction = ScriptableObject.CreateInstance("Rotate") as Action;
        this.constructionController.currentState = ConstructionController.ConstructionState.Editing;
        this.constructionController.editionState = ConstructionController.EditionType.Rotation;

        this.currentAction.Initialize(selection.GetGameObject());
        this.constructionController.SetGhost(selection.GetGameObject().GetComponent<GhostHandler>());
    }
}
