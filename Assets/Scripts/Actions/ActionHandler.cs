using UnityEngine;

[CreateAssetMenu]
public class ActionHandler : ScriptableObject
{
    public Action currentAction = null;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;

    private ConstructionController constructionController;

    public void Initialize()
    {
        this.revertActionSet.items.Clear();
        this.redoActionSet.items.Clear();
        this.constructionController = ConstructionController.instance;
    }

    public void NewStateAction(string action, GameObject gameObject, bool updateState = true)
    {
        redoActionSet.ClearSet();
        CreateAction(action, gameObject);
        ActionComplete(updateState);
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

    public void ActionComplete(bool updateState)
    {
        this.currentAction.Complete();
        revertActionSet.Add(currentAction);
        this.currentAction = null;

        if (updateState)
        {
            this.constructionController.editionState = ConstructionController.EditionType.Off;//TODO TMP?
            this.constructionController.currentState = ConstructionController.ConstructionState.Off;
        }
        ReactProxy.instance.unsavedWork = true;
    }

    public Action RedoAction()
    {
        bool shouldSelect;
        Action action = this.redoActionSet.GetLastAction();

        if (action != null)
        {
            shouldSelect = action.ReDo();
            this.revertActionSet.Add(action);
            this.redoActionSet.Remove(action);
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
        Action action = this.revertActionSet.GetLastAction();

        if (action != null)
        {
            shouldSelect = action.Revert();
            this.redoActionSet.Add(action);
            this.revertActionSet.Remove(action);
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
