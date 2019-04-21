using UnityEngine;

public class Move : Action
{
    private Vector3 oldPosition;
    private Vector3 newPosition;

    public override void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.oldPosition = gameObject.transform.position;
    }

    public override void Complete()
    {
        base.Complete();
        this.newPosition = this.gameObject.transform.position;
    }

    public override bool Revert()
    {
        GhostHandler ghost = this.gameObject.GetComponent<GhostHandler>();
        if (ghost != null)
            ghost.Move(this.oldPosition);
        else
            this.gameObject.transform.position = this.oldPosition;
        return true;
    }

    public override bool ReDo()
    {
        GhostHandler ghost = this.gameObject.GetComponent<GhostHandler>();
        if (ghost != null)
            ghost.Move(this.newPosition);
        else
            this.gameObject.transform.position = this.newPosition;
        return true;
    }
}