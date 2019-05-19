using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShapeCreator : GhostHandler
{
    public UnityEvent eventShapeConstructionFinished = new UnityEvent();
    public ShapePoint pointPrefab;
    public List<ShapePoint> points { get; } = new List<ShapePoint>();
    public ShapePoint currentPoint = null;
    public float minStep = 25;
    public FlowerBed flowerBed;

    private LineTextHandler lineDistanceText;
    private LineTextHandler lineAngleText;
    private ShapePoint firstPoint = null;
    private Vector2 a, b, c;
    private Color color = new Color(1f, 0f, 0f, 1f);

    public void Init()
    {
        GridController.instance.eventPostRender.AddListener(DrawLines);
        transform.position = new Vector3(0, 0, 0);
        firstPoint = Instantiate(pointPrefab);
        firstPoint.ChangeColor(new Color(0f, 0f, 1f, 1f));
        points.Add(firstPoint);
        currentPoint = firstPoint;
        lineDistanceText = Instantiate<LineTextHandler>(SpawnController.instance.lineText);
        lineAngleText = Instantiate<LineTextHandler>(SpawnController.instance.lineText);
        lineDistanceText.gameObject.SetActive(false);
        lineAngleText.gameObject.SetActive(false);
    }

    public void SelfClear()
    {
        Destroy(firstPoint.gameObject);
        Destroy(currentPoint.gameObject);
        foreach (ShapePoint point in points)
            Destroy(point.gameObject);
        points.Clear();
        lineDistanceText.gameObject.SetActive(false);
        lineAngleText.gameObject.SetActive(false);
    }

    //GhostHandler overrides
    public override void Positioning(Vector3 position)
    {
        currentPoint.transform.position = new Vector3(position.x, 0.2f, position.z);
        if (lineDistanceText.isActiveAndEnabled)
        {
            Vector3 p1 = points[points.Count - 1].transform.position;
            Vector3 p2 = points[points.Count - 2].transform.position;

            lineDistanceText.transform.position = (p1 + p2) / 2f;
            lineDistanceText.SetText(string.Format("{0:F1}m", (p1 - p2).magnitude));

            if (lineAngleText.isActiveAndEnabled)
            {
                c = new Vector2(p1.x, p1.z);
                lineAngleText.SetText(string.Format("{0:F1}°", Vector2.Angle(a - b, c - b)));
                lineAngleText.transform.position = p2 + (p1 - p2).normalized - (p2 - points[points.Count - 3].transform.position).normalized;
            }
        }
    }

    public override bool FromPositioningToBuilding(Vector3 position)
    {
        if (points.Count == 1)
        {
            if (ConstructionController.instance.lastCastHit.collider.gameObject.tag == "FlowerBed")
                return MessageHandler.instance.ErrorMessage("shape_creator", "elements_overlap");
            CreatePoint();
            lineDistanceText.gameObject.SetActive(true);
            return false;
        }

        if (CheckIntersectWithOtherObjects(points[points.Count - 2].transform.position, position))
            return MessageHandler.instance.ErrorMessage("shape_creator", "elements_overlap");

        if (currentPoint != firstPoint
          && Vector2.Distance(new Vector2(firstPoint.transform.position.x, firstPoint.transform.position.z), new Vector2(position.x, position.z)) < 0.5f)
        {
            if (points.Count < 4)
                return MessageHandler.instance.ErrorMessage("shape_creator", "lessthan_3points");

            Vector3 tmp = currentPoint.transform.position;
            currentPoint.transform.position = firstPoint.transform.position;
            points.Remove(currentPoint);
            if (!CheckContainOtherObjects())
            {
                currentPoint.transform.position = tmp;
                points.Add(currentPoint);
                return MessageHandler.instance.ErrorMessage("shape_creator", "elements_overlap");
            }
            return true;
        }

        if (points.Count > 3 && CheckIntersection(new Vector2(position.x, position.z)))
            return MessageHandler.instance.ErrorMessage("shape_creator", "line_cross");

        if (points.Count > 1 && Vector3.Distance(position, points[points.Count - 1].transform.position) < 0.1f)
            return false;

        CreatePoint();
        lineAngleText.gameObject.SetActive(true);
        a = new Vector2(points[points.Count - 3].transform.position.x, points[points.Count - 3].transform.position.z);
        b = new Vector2(points[points.Count - 2].transform.position.x, points[points.Count - 2].transform.position.z);
        return false;
    }

    public override bool Building(Vector3 position) { return true; }

    public override void EndConstruction(Vector3 position)
    {
        GridController.instance.eventPostRender.RemoveListener(DrawLines);
        lineDistanceText.gameObject.SetActive(false);
        lineAngleText.gameObject.SetActive(false);
        Destroy(currentPoint);
        eventShapeConstructionFinished.Invoke();
    }

    public override bool OnCancel()
    {
        GridController.instance.eventPostRender.RemoveListener(DrawLines);
        for (int i = points.Count - 1; i >= 0; i--)
        {
            Destroy(points[i].gameObject);
            points.RemoveAt(i);
        }
        Destroy(flowerBed.gameObject);
        lineDistanceText.gameObject.SetActive(false);
        lineAngleText.gameObject.SetActive(false);
        return false;
    }

    //Actions
    public bool AddPoint(ShapePoint point)
    {
        points.Remove(currentPoint);
        points.Add(point);
        points.Add(currentPoint);

        if (points.Count == 2)
        {
            currentPoint.Activate();
            gameObject.SetActive(true);
            GridController.instance.eventPostRender.AddListener(DrawLines);
        }
        return true;
    }

    public bool RemovePoint(ShapePoint point)
    {
        points.Remove(point);
        if (points.Count == 1)
        {
            currentPoint.DeActivate();
            gameObject.SetActive(false);
            GridController.instance.eventPostRender.RemoveListener(DrawLines);
            return false;
        }
        return true;
    }


    private void CreatePoint()
    {
        Vector3 position;
        RaycastHit hit;
        ShapePoint tmp;

        if (ConstructionController.instance.MouseRayCast(out position, out hit))
        {
            currentPoint.EndConstruction();
            PlayerController.instance.actionHandler.NewStateAction("AddLineShape", gameObject, false);
            tmp = Instantiate(pointPrefab, transform);
            tmp.transform.position = new Vector3(position.x, 0.2f, position.z);
            points.Add(tmp);
            currentPoint = tmp;
        }
    }

    //Collision Detection
    private bool CheckContainOtherObjects()
    {
        flowerBed.OnShapeFinished();
        MeshCollider collider = flowerBed.GetComponent<MeshCollider>();
        foreach (FlowerBed fb in ConstructionController.instance.flowerBeds)
            foreach (Vector2 point in fb.vertices)
                if (pointInPolygon(flowerBed.vertices, point.x, point.y))
                {
                    flowerBed.ActivationCancel();
                    return false;
                }
        return true;
    }

    private bool pointInPolygon(Vector2[] pointList, float x, float y)
    {
        int polyCorners = pointList.Length;
        int i, j = polyCorners - 1;
        bool oddNodes = false;

        for (i = 0; i < polyCorners; i++)
        {
            if ((pointList[i].y < y && pointList[j].y >= y || pointList[j].y < y && pointList[i].y >= y) && (pointList[i].x <= x || pointList[j].x <= x))
            {
                if (pointList[i].x + (y - pointList[i].y) / (pointList[j].y - pointList[i].y) * (pointList[j].x - pointList[i].x) < x)
                    oddNodes = !oddNodes;
            }
            j = i;
        }

        return oddNodes;
    }

    private bool CheckIntersectWithOtherObjects(Vector3 start, Vector3 end)
    {
        Vector3 s = new Vector3(start.x, 0.2f, start.z);
        Vector3 e = new Vector3(end.x, 0.2f, end.z);
        Vector3 center = (s + e) / 2f;
        RaycastHit[] hit;

        hit = Physics.BoxCastAll(
            center,
            new Vector3(Vector3.Distance(s, e) / 2f, 0.3f, 0.01f),
            new Vector3(0f, -1f, 0f),
            Quaternion.LookRotation(e - s, Vector3.up) * Quaternion.Euler(0, 90, 0));

        foreach (RaycastHit elem in hit)
            if (elem.collider.tag == "FlowerBed" || elem.collider.tag == "Invalid")
                return true;

        return false;
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

    //Visual
    private void DrawLines()
    {
        if (points.Count < 1)
            return;
        ShapePoint tmp = firstPoint;
        foreach (ShapePoint point in points)
        {
            GridController.instance.DrawLimited(tmp.transform.position, point.transform.position, color);
            tmp = point;
        }
    }
}
