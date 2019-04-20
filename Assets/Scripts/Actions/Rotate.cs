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

    public override void Revert()
    {
        this.gameObject.transform.rotation = this.oldRotation;
    }

    public override void ReDo()
    {
        this.gameObject.transform.rotation = this.newRotation;
    }
}