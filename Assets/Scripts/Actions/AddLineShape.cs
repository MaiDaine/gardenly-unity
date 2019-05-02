using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddLineShape : GhostAction
{
    private ShapePoint point;

    public override void Initialize(GameObject gameObject)
    {
        base.Initialize(gameObject);
        tmpAction = true;
    }

    public override void Complete()
    {
        this.point = this.gameObject.GetComponent<ShapeCreator>().currentPoint;
    }

    public override bool Revert()
    {
        this.point.DeActivate();
        this.gameObject.GetComponent<ShapeCreator>().RemovePoint(point);
        return false;
    }

    public override bool ReDo()
    {
        this.point.Activate();
        this.gameObject.GetComponent<ShapeCreator>().AddPoint(point);
        return false;
    }

    private void OnDestroy()
    {
        if (point != null && !point.isActiveAndEnabled)
            Destroy(this.point);
    }
}
