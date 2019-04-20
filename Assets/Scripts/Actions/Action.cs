using UnityEngine;

public abstract class Action : ScriptableObject
{
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

    public abstract void Revert();

    public abstract void ReDo();

    public virtual GameObject GetGameObject() { return this.gameObject; } 
}
