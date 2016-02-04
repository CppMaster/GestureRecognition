using UnityEngine;
using System.Collections.Generic;

public class GestureRecognizer : MonoBehaviour
{

    protected List<Vector2>[] pointArray;
    public int maxPoints = 64;
    protected int gestureChosen = -1;


    public virtual int StartRecognizer(List<Vector2> points)
    {
        return -1;
    }

    public virtual List<Vector2> GetStepPoints(int _step)
    {
        return pointArray[_step];
    }

    public virtual int GetStepCount()
    {
        return pointArray.Length;
    }

    public List<Vector2> OptimizeGesture(List<Vector2> points, int maxPoints)
    {
        float interval = CalcTotalGestureLength(points) / (maxPoints - 1);

        List<Vector2> optimizedPoints = new List<Vector2>();
        optimizedPoints.Add(points[0]);

        float tempDistance = 0f;
        for (int i = 1; i < points.Count; ++i)
        {
            float currentDistanceBetween2Ponts = CalcDistance(points[i - 1], points[i]);

            if (tempDistance + currentDistanceBetween2Ponts >= interval)
            {
                Vector2 newPoint = new Vector2(
                    points[i - 1].x + ((interval - tempDistance) / currentDistanceBetween2Ponts) * (points[i].x - points[i - 1].x),
                    points[i - 1].y + ((interval - tempDistance) / currentDistanceBetween2Ponts) * (points[i].y - points[i - 1].y)
                    );

                optimizedPoints.Add(newPoint);

                points.Insert(i, newPoint);

                tempDistance = 0f;
            }
            else
            {
                tempDistance += currentDistanceBetween2Ponts;
            }
        }

        if (optimizedPoints.Count == maxPoints - 1)
        {
            optimizedPoints.Add(points[points.Count - 1]);
        }
        return optimizedPoints;

    }

    protected float CalcTotalGestureLength(List<Vector2> points)
    {
        float length = 0f;
        for (int a = 1; a < points.Count; ++a)
        {
            length += CalcDistance(points[a - 1], points[a]);
        }
        return length;
    }

    protected float CalcDistance(Vector2 p1, Vector2 p2)
    {
        return (p1 - p2).magnitude;
    }

}
