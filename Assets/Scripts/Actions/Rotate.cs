using UnityEngine;

public class Rotate : GhostAction
{
    private Quaternion oldRotation;
    private Quaternion newRotation;

    public override void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        oldRotation = gameObject.transform.rotation;
    }

    public override bool Complete()
    {
        base.Complete();
        newRotation = gameObject.transform.rotation;
        return true;
    }

    public override bool Revert()
    {
        gameObject.transform.rotation = oldRotation;
        return true;
    }

    public override bool ReDo()
    {
        gameObject.transform.rotation = newRotation;
        return true;
    }
}