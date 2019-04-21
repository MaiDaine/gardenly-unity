using UnityEngine;

public class Rotate : Action
{
    private Quaternion oldRotation;
    private Quaternion newRotation;

    public override void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.oldRotation = gameObject.transform.rotation;
    }

    public override void Complete()
    {
        base.Complete();
        this.newRotation = this.gameObject.transform.rotation;
    }

    public override bool Revert()
    {
        this.gameObject.transform.rotation = this.oldRotation;
        return true;
    }

    public override bool ReDo()
    {
        this.gameObject.transform.rotation = this.newRotation;
        return true;
    }
}