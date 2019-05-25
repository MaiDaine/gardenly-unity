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
        point = gameObject.GetComponent<ShapeCreator>().currentPoint;
    }

    public override bool Revert()
    {
        point.DeActivate();
        gameObject.GetComponent<ShapeCreator>().RemovePoint(point);
        return false;
    }

    public override bool ReDo()
    {
        point.Activate();
        gameObject.GetComponent<ShapeCreator>().AddPoint(point);
        return false;
    }

    private void OnDestroy()
    {
        if (point != null && !point.isActiveAndEnabled)
            Destroy(point);
    }
}
