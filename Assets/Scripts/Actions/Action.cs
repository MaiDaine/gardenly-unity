using UnityEngine;

public abstract class Action : ScriptableObject
{
    protected GameObject gameObject;

    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public abstract void Complete();

    public abstract void Revert();

    public abstract void ReDo();

    public virtual GameObject GetGameObject() { return this.gameObject; } 
}
