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
        this.constructionController.editionState = ConstructionController.EditionType.Off;//TODO TMP?
        this.constructionController.currentState = ConstructionController.ConstructionState.Off;
    }

    public void CreateAction(string action, ISelectable selection)
    {
        this.currentAction = ScriptableObject.CreateInstance(action) as Action;
        this.currentAction.Initialize(selection.GetGameObject());
    }

    public void CreateAction(string action, GameObject gameObject)
    {
        this.currentAction = ScriptableObject.CreateInstance(action) as Action;
        this.currentAction.Initialize(gameObject);
    }
}
