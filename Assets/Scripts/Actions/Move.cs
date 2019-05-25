using UnityEngine;

public class Move : GhostAction
{
    private Vector3 oldPosition;
    private Vector3 newPosition;

    public override void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        oldPosition = gameObject.transform.position;
    }

    public override void Complete()
    {
        base.Complete();
        newPosition = gameObject.transform.position;
    }

    public override bool Revert()
    {
        GhostHandler ghost = gameObject.GetComponent<GhostHandler>();
        if (ghost != null)
            ghost.Move(oldPosition);
        else
            gameObject.transform.position = oldPosition;
        return true;
    }

    public override bool ReDo()
    {
        GhostHandler ghost = gameObject.GetComponent<GhostHandler>();
        if (ghost != null)
            ghost.Move(newPosition);
        else
            gameObject.transform.position = newPosition;
        return true;
    }
}