using UnityEngine;
using System.Collections.Generic;

public class GestureRecognizerSimplifier : GestureRecognizer
{

    public float minAngle = 30f;
    public float minEdgeRatio = 0.1f;

    public List<Vector2> RemoveSmallAngles(List<Vector2> points)
    {

        List<Vector2> result = new List<Vector2>();
        result.Add(points[0]);
        result.Add(points[1]);
        for (int a = 2; a < points.Count; ++a)
        {
            Vector2 translation1 = result[result.Count - 1] - result[result.Count - 2];
            Vector2 translation2 = points[a] - result[result.Count - 1];
            float angle1 = Mathf.Atan2(translation1.y, translation1.x) * Mathf.Rad2Deg;
            float angle2 = Mathf.Atan2(translation2.y, translation2.x) * Mathf.Rad2Deg;
            float angleDiff = Mathf.DeltaAngle(angle1, angle2);
            if (Mathf.Abs(angleDiff) < minAngle)
            {
                result.RemoveAt(result.Count - 1);
            }
            result.Add(points[a]);
        }

        return result;
    }

    public List<Vector2> RemoveSmallEdges(List<Vector2> points)
    {
        float minEdgeLength = CalcTotalGestureLength(points) * minEdgeRatio;

        List<Vector2> result = new List<Vector2>();
        result.Add(points[0]);
        for (int a = 1; a < points.Count; ++a)
        {
            float distance = CalcDistance(points[a], result[result.Count - 1]);
            if (distance >= minEdgeLength)
            {
                result.Add(points[a]);
            }
            else
            {
                result.Add((points[a] + result[result.Count - 1]) / 2);
                result.Remove(result[result.Count - 2]);
            }
        }

        return result;
    }

    public override int StartRecognizer(List<Vector2> points)
    {
        pointArray = new List<Vector2>[3];
        pointArray[0] = OptimizeGesture(points, maxPoints);
        pointArray[1] = RemoveSmallAngles(pointArray[0]);
        pointArray[2] = RemoveSmallEdges(pointArray[1]);
        return base.StartRecognizer(points);
    }
}
