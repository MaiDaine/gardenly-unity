using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHandler : ScriptableObject
{
    public Action currentAction = null;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;

    private ConstructionController constructionController;

    public void Initialize(ActionRuntimeSet revertActionSet, ActionRuntimeSet redoActionSet)
    {
        this.revertActionSet = revertActionSet;
        this.redoActionSet = redoActionSet;
        this.constructionController = ConstructionController.instance;
    }

    public void NewStateAction(string action, GameObject gameObject)
    {
        redoActionSet.ClearSet();
        CreateAction(action, gameObject);
        ActionComplete(revertActionSet);
    }

    public void NewEditonAction(ConstructionController.EditionType type, ISelectable currentSelection)
    {
        constructionController.currentState = ConstructionController.ConstructionState.Editing;
        redoActionSet.ClearSet();

        switch (type)
        {
            case ConstructionController.EditionType.Position:
                {
                    this.constructionController.currentState = ConstructionController.ConstructionState.Editing;
                    this.constructionController.editionState = ConstructionController.EditionType.Position;
                    CreateAction("Move", currentSelection);
                    this.constructionController.SetGhost(currentSelection.GetGameObject().GetComponent<GhostHandler>());
                    break;
                }
            case ConstructionController.EditionType.Rotation:
                {
                    this.constructionController.currentState = ConstructionController.ConstructionState.Editing;
                    this.constructionController.editionState = ConstructionController.EditionType.Rotation;
                    CreateAction("Rotate", currentSelection);
                    this.constructionController.SetGhost(currentSelection.GetGameObject().GetComponent<GhostHandler>());
                    break;
                }
            default:
                break;
        }
        constructionController.SetGhost(currentSelection.GetGameObject().GetComponent<GhostHandler>());//TODO might change
    }

    public void ActionComplete(ActionRuntimeSet revertActionSet)
    {
        this.currentAction.Complete();
        revertActionSet.Add(currentAction);
        this.currentAction = null;
        this.constructionController.editionState = ConstructionController.EditionType.Off;//TODO TMP?
        this.constructionController.currentState = ConstructionController.ConstructionState.Off;
    }

    public Action RedoAction()
    {
        bool shouldSelect;
        Action action = redoActionSet.GetLastAction();

        if (action != null)
        {
            shouldSelect = action.ReDo();
            revertActionSet.Add(action);
            redoActionSet.Remove(action);
            if (shouldSelect)
                return action;
        }
        else
            ErrorHandler.instance.ErrorMessage("No action to cancel");
        return null;
    }

    public Action RevertAction()
    {
        bool shouldSelect;
        Action action = revertActionSet.GetLastAction();

        if (action != null)
        {
            shouldSelect = action.Revert();
            redoActionSet.Add(action);
            revertActionSet.Remove(action);
            if (shouldSelect)
                return action;
        }
        else
            ErrorHandler.instance.ErrorMessage("No action to revert");
        return null;
    }

    private void CreateAction(string action, ISelectable selection)
    {
        this.currentAction = ScriptableObject.CreateInstance(action) as Action;
        this.currentAction.Initialize(selection.GetGameObject());
    }

    private void CreateAction(string action, GameObject gameObject)
    {
        this.currentAction = ScriptableObject.CreateInstance(action) as Action;
        this.currentAction.Initialize(gameObject);
    }
}
