using UnityEngine;


[CreateAssetMenu(menuName = "RuntimeSet/Actions")]
public class ActionRuntimeSet : RuntimeSet<Action>
{
    public Action GetLastAction()
    {
        if (items.Count == 0)
            return null;
        return (items[items.Count - 1]);
    }
}
