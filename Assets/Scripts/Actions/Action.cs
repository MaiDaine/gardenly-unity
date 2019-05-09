using UnityEngine;

public abstract class GhostAction : ScriptableObject
{
    public bool tmpAction = false;
    protected GameObject gameObject;
    
    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public virtual void Complete()
    {
        UIController uIController = Camera.main.GetComponent<UIController>();

        if (uIController != null)
            uIController.GetMenuScript().EditionEnd();
    }

    public abstract bool Revert();//Return true if object needs to be selected

    public abstract bool ReDo();//Return true if object needs to be selected

    public virtual GameObject GetGameObject() { return this.gameObject; } 
}
