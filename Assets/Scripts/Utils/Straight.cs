using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Straight : MonoBehaviour
{
    //y = ax + b
    private float a;
    private float b;
    private Vector2 check;

    public void UpdateEquation(Vector2 start, Vector2 end)
    {
        check = start;
        if (start.x == end.x)
        {
            a = 0;
            check.y = 0;
        }
        else
            a = (end.y - start.y) / (end.x - start.x);
        if (start.y == end.y)
            check.x = 0;
        b = start.y - (a * start.x);
    }

    public float CheckPointPosition(Vector2 point, out Vector2 linePoint, bool useX = false)
    {
        if (a == 0)
        {
            if (check.x != 0)
            {
                linePoint = new Vector2(check.x, point.y);
                if (check.x < point.x)
                    return -1;
                else if (check.x == point.x)
                    return 0;
            }
            else
            {
                linePoint = new Vector2(point.x, check.y);
                if (check.y < point.y)
                    return -1;
                else if (check.y == point.y)
                    return 0;
            }
            return 1;
        }

        if (useX)
            linePoint = new Vector2(point.x, (a * point.x) + b);
        else
            linePoint = new Vector2(((point.y - b) / a), point.y);
        return (point.x - linePoint.x);
    }
}
