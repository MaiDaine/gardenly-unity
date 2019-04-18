using UnityEngine;


[CreateAssetMenu(menuName = "RuntimeSet/Actions")]
public class ActionRuntimeSet : RuntimeSet<Action>
{
    public GameEvent onActionSetUpdated;

    public override void Add(Action elem)
    {
        if (!items.Contains(elem))
        {
            items.Add(elem);
            onActionSetUpdated.Raise();
        }
    }

    public override void Remove(Action elem)
    {
        if (items.Contains(elem))
        {
            items.Remove(elem);
            onActionSetUpdated.Raise();
        }
    }

    public override void ClearSet()
    {
        if (items.Count != 0)
        {
            items.Clear();
            onActionSetUpdated.Raise();
        }
    }

    public Action GetLastAction()
    {
        if (items.Count == 0)
            return null;
        return (items[items.Count - 1]);
    }
}
