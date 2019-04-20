using UnityEngine;

[CreateAssetMenu(menuName = "RuntimeSet/Actions")]
public class ActionRuntimeSet : RuntimeSet<Action>
{
    public GameEvent onActionSetUpdated;

    public override void Add(Action elem)
    {
        if (!this.items.Contains(elem))
        {
            this.items.Add(elem);
            onActionSetUpdated.Raise();
        }
    }

    public override void Remove(Action elem)
    {
        if (this.items.Contains(elem))
        {
            this.items.Remove(elem);
            onActionSetUpdated.Raise();
        }
    }

    public override void ClearSet()
    {
        if (this.items.Count != 0)
        {
            this.items.Clear();
            onActionSetUpdated.Raise();
        }
    }

    public Action GetLastAction()
    {
        if (this.items.Count == 0)
            return null;
        return (this.items[this.items.Count - 1]);
    }
}