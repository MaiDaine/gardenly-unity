using UnityEngine;

[CreateAssetMenu(menuName = "RuntimeSet/Actions")]
public class ActionRuntimeSet : RuntimeSet<GhostAction>
{
    public GameEvent onActionSetUpdated;

    public override void Add(GhostAction elem)
    {
        if (!this.items.Contains(elem))
        {
            this.items.Add(elem);
            onActionSetUpdated.Raise();
        }
    }

    public override void Remove(GhostAction elem)
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
            foreach (GhostAction action in items)
                Destroy(action);
            items.Clear();
            onActionSetUpdated.Raise();
        }
    }

    public void ClearTmpAction()
    {
        for (int i = items.Count - 1; i > -1; i--)
            if (items[i].tmpAction)
            {
                Destroy(items[i]);
                items.RemoveAt(i);
            }
    }

    public GhostAction GetLastAction()
    {
        if (this.items.Count == 0)
            return null;
        return (this.items[this.items.Count - 1]);
    }
}