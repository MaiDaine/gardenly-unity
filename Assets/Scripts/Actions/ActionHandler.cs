using UnityEngine;

[CreateAssetMenu]
public class ActionHandler : ScriptableObject
{
    public GhostAction currentAction = null;
    public ActionRuntimeSet revertActionSet;
    public ActionRuntimeSet redoActionSet;

    private ConstructionController constructionController;

    public void Initialize()
    {
        revertActionSet.items.Clear();
        redoActionSet.items.Clear();
        constructionController = ConstructionController.instance;
    }

    public void NewStateAction(string action, GameObject gameObject, bool updateState = true)
    {
        redoActionSet.ClearSet();
        CreateAction(action, gameObject);
        ActionComplete(updateState);
    }

    // FIXME -> typo: NewEdit[i]onAction
    public void NewEditonAction(ConstructionController.EditionType type, ISelectable currentSelection)
    {
        constructionController.currentState = ConstructionController.ConstructionState.Editing;
        redoActionSet.ClearSet();
        currentSelection.GetGameObject().GetComponent<GhostHandler>().StartAction();
        switch (type)
        {
            case ConstructionController.EditionType.Position:
                {
                    constructionController.currentState = ConstructionController.ConstructionState.Editing;
                    constructionController.editionState = ConstructionController.EditionType.Position;
                    CreateAction("Move", currentSelection);
                    constructionController.SetGhost(currentSelection.GetGameObject().GetComponent<GhostHandler>());
                    break;
                }
            case ConstructionController.EditionType.Rotation:
                {
                    constructionController.currentState = ConstructionController.ConstructionState.Editing;
                    constructionController.editionState = ConstructionController.EditionType.Rotation;
                    CreateAction("Rotate", currentSelection);
                    constructionController.SetGhost(currentSelection.GetGameObject().GetComponent<GhostHandler>());
                    break;
                }
            default:
                break;
        }
        constructionController.SetGhost(currentSelection.GetGameObject().GetComponent<GhostHandler>());//TODO might change
    }

    public void ActionComplete(bool updateState)
    {
        if (ConstructionController.instance.currentState == ConstructionController.ConstructionState.Editing)
            PlayerController.instance.currentSelection.GetGameObject().GetComponent<GhostHandler>().EndAction();
        if (currentAction.Complete())
            ReactProxy.instance.UpdateSaveState(true);
        revertActionSet.Add(currentAction);
        currentAction = null;

        if (updateState)
        {
            constructionController.editionState = ConstructionController.EditionType.Off;//TODO TMP?
            constructionController.currentState = ConstructionController.ConstructionState.Off;
        }
    }

    public GhostAction RedoAction()
    {
        bool shouldSelect;
        GhostAction action = redoActionSet.GetLastAction();

        if (action != null)
        {
            shouldSelect = action.ReDo();
            revertActionSet.Add(action);
            redoActionSet.Remove(action);
            if (shouldSelect)
                return action;
        }
        else
            MessageHandler.instance.ErrorMessage("action", "no_cancel");
        return null;
    }

    public GhostAction RevertAction()
    {
        bool shouldSelect;
        GhostAction action = revertActionSet.GetLastAction();

        if (action != null)
        {
            shouldSelect = action.Revert();
            redoActionSet.Add(action);
            revertActionSet.Remove(action);
            if (shouldSelect)
                return action;
        }
        else
            MessageHandler.instance.ErrorMessage("action", "no_revert");
        return null;
    }

    private void CreateAction(string action, ISelectable selection)
    {
        currentAction = ScriptableObject.CreateInstance(action) as GhostAction;
        currentAction.Initialize(selection.GetGameObject());
    }

    private void CreateAction(string action, GameObject gameObject)
    {
        currentAction = ScriptableObject.CreateInstance(action) as GhostAction;
        currentAction.Initialize(gameObject);
    }
}
