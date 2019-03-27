using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShapeCreator : GhostHandler
{
    public UnityEvent eventShapeConstructionFinished = new UnityEvent();
    public ShapePoint pointPrefab;
    public List<ShapePoint> points { get; } = new List<ShapePoint>();

    private ShapePoint firstPoint = null;
    private ShapePoint currentPoint = null;
    private Color color = new Color(1f, 0f, 0f, 1f);

    public void Init()
    {
        GridController.instance.eventPostRender.AddListener(DrawLines);
        this.transform.position = new Vector3(0, 0, 0);
        this.firstPoint = Instantiate(pointPrefab);
        this.points.Add(firstPoint);
        this.currentPoint = firstPoint;
    }

    public void SelfClear()
    {
        Destroy(this.firstPoint.gameObject);
        Destroy(this.currentPoint.gameObject);
        foreach (ShapePoint point in points)
            Destroy(point.gameObject);
        this.points.Clear();
    }

    //GhostHandler overrides
    public override void Positioning(Vector3 position)
    {
        this.currentPoint.transform.position = new Vector3(position.x, 0.2f, position.z);
    }

    public override bool FromPositioningToBuilding(Vector3 position)
    {
        if (this.currentPoint != this.firstPoint
          && Vector2.Distance(new Vector2(this.firstPoint.transform.position.x, this.firstPoint.transform.position.z), new Vector2(position.x, position.z)) < 1.5f)
        {
            if (this.points.Count < 4)
            {
                ErrorHandler.instance.ErrorMessage("Place at least 3 points");
                return false;
            }
            return true;
        }
        if (this.points.Count > 3 && CheckIntersection(new Vector2(position.x, position.z)))
        {
            ErrorHandler.instance.ErrorMessage("You can't cross the lines");
            return false;
        }
        if (this.points.Count > 1 && Vector3.Distance(position, this.points[this.points.Count - 1].transform.position) < 0.1f)
            return false;
        CreatePoint();
        return false;
    }

    public override bool Building(Vector3 position) { return true; }

    public override void EndConstruction(Vector3 position)
    {
        GridController.instance.eventPostRender.RemoveListener(DrawLines);
        this.eventShapeConstructionFinished.Invoke();
    }


    private void DrawLines()
    {
        if (this.points.Count < 1)
            return;
        ShapePoint tmp = firstPoint;
        foreach (ShapePoint point in points)
        {
            GridController.instance.DrawLimited(tmp.transform.position, point.transform.position, color);
            tmp = point;
        }
    }

    private void CreatePoint()
    {
        Vector3 position;
        RaycastHit hit;
        ShapePoint tmp;

        if (ConstructionController.instance.MouseRayCast(out position, out hit))
        {
            this.currentPoint.EndConstruction();
            tmp = Instantiate(pointPrefab, this.transform);
            tmp.transform.position = new Vector3(position.x, 0.2f, position.z);
            this.points.Add(tmp);
            this.currentPoint = tmp;
        }
    }

    private bool CheckIntersection(Vector2 p4)
    {
        Vector2 p3 = new Vector2(points[points.Count - 2].transform.position.x, points[points.Count - 2].transform.position.z);

        for (int i = 0; i < points.Count - 3; i++)
        {
            Vector2 p1 = new Vector2(points[i].transform.position.x, points[i].transform.position.z);
            Vector2 p2 = new Vector2(points[i + 1].transform.position.x, points[i + 1].transform.position.z);

            var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

            if (d != 0.0f)
            {
                var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
                var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;
                if (!(u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f))
                    return true;
            }
        }
        return false;
    }
}
