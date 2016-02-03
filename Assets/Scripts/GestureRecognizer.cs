using UnityEngine;
using System.Collections.Generic;

public class GestureRecognizer : MonoBehaviour
{

    public List<Vector2>[] pointArray = new List<Vector2>[5];
    public int maxPoints = 64;
    public float sizeOfScaleRect = 500f;
    public int compareDetail = 15;
    public float angleRange = 45f;
    int gestureChosen = -1;


    public int StartRecognizer(List<Vector2> points)
    {
        pointArray[0] = points;
        pointArray[1] = OptimizeGesture(points, maxPoints);
        Vector2 center = CalcCenterOfGesture(pointArray[1]);
        float radians = Mathf.Atan2(center.y - pointArray[1][0].y, center.x - pointArray[1][0].x);
        pointArray[2] = RotateGesture(pointArray[1], -radians, center);
        pointArray[3] = ScaleGesture(pointArray[2], sizeOfScaleRect);
        pointArray[4] = TranslateGestureToOrigin(pointArray[3]);
        gestureChosen = GestureMatch(pointArray[4]);
        return gestureChosen;
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

    public List<Vector2> RotateGesture(List<Vector2> points, float radians, Vector2 center)
    {
        /*List<Vector2> newArray = new List<Vector2>();
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        for (int i = 0; i < points.Count; ++i)
        {
            newArray.Add(new Vector2(
                (points[i].x - center.x) * cos - (points[i].y - center.y) * sin + center.x,
                (points[i].x - center.x) * sin - (points[i].y - center.y) * cos + center.y
                ));
        }

        return newArray; */
        return points;
    }

    public List<Vector2> ScaleGesture(List<Vector2> points, float size)
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;

        for (int i = 0; i < points.Count; ++i)
        {
            minX = Mathf.Min(minX, points[i].x);
            maxX = Mathf.Max(maxX, points[i].x);
            minY = Mathf.Min(minY, points[i].y);
            maxY = Mathf.Max(maxY, points[i].y);
        }

        Rect boundingBox = new Rect(minX, minY, maxX - minX, maxY - minY);

        List<Vector2> newArray = new List<Vector2>();

        for (int i = 0; i < points.Count; ++i)
        {
            newArray.Add(new Vector2(
                points[i].x * (size / boundingBox.width),
                points[i].y * (size / boundingBox.height)
                ));
        }

        return newArray;
    }

    public List<Vector2> TranslateGestureToOrigin(List<Vector2> points)
    {

        Vector2 center = CalcCenterOfGesture(points);

        List<Vector2> newArray = new List<Vector2>();

        for (int i = 0; i < points.Count; ++i)
        {
            newArray.Add(new Vector2(
                points[i].x - center.x,
                points[i].y - center.y
                ));
        }

        return newArray;
    }

    public int GestureMatch(List<Vector2> points)
    {

        float tempDistance = Mathf.Infinity;
        int index = 0;

        for (int a = 0; a < GestureTemplates.templates.Length; ++a)
        {
            float distance = CalcDistanceAtOptimalAngle(points, new List<Vector2>(GestureTemplates.templates[a]), -angleRange, angleRange);

            if (distance < tempDistance)
            {
                tempDistance = distance;
                index = a;
            }
        }

        float halfDiagonal = 0.5f * Mathf.Pow(2, 0.5f) * sizeOfScaleRect;
        float score = 1f - tempDistance / halfDiagonal;

        if (score < 0.1f)
        {
            Debug.Log("No match! Score: " + score);
            return -1;
        }

        Debug.Log("Result: " + GestureTemplates.templateNames[index] + ", Score:" + score);
        return index;


    }

    public Vector2 CalcCenterOfGesture(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;

        for (int a = 0; a < points.Count; ++a)
        {
            sum += points[a];
        }

        return sum /= points.Count;
    }

    public float CalcDistance(Vector2 p1, Vector2 p2)
    {
        return (p1 - p2).magnitude;
    }

    public float CalcTotalGestureLength(List<Vector2> points)
    {
        float length = 0f;
        for (int a = 1; a < points.Count; ++a)
        {
            length += CalcDistance(points[a - 1], points[a]);
        }
        return length;
    }

    public float CalcDistanceAtOptimalAngle(List<Vector2> points, List<Vector2> template, float negativeAngle, float positiveAngle)
    {

        float radian1 = Mathf.PI * negativeAngle + (1f - Mathf.PI) * positiveAngle;
        float tempDistance1 = CalcDistanceAtAngle(points, template, radian1);

        float radian2 = Mathf.PI * positiveAngle + (1f - Mathf.PI) * negativeAngle;
        float tempDistance2 = CalcDistanceAtAngle(points, template, radian2);

        for (int a = 0; a < compareDetail; ++a)
        {
            if (tempDistance1 < tempDistance2)
            {
                positiveAngle = radian2;
                radian2 = radian1;
                tempDistance2 = tempDistance1;
                radian1 = Mathf.PI * negativeAngle + (1f - Mathf.PI) * positiveAngle;
                tempDistance1 = CalcDistanceAtAngle(points, template, radian1);
            }
            else
            {
                negativeAngle = radian1;
                radian1 = radian2;
                tempDistance1 = tempDistance2;
                radian2 = Mathf.PI * positiveAngle + (1f - Mathf.PI) * negativeAngle;
                tempDistance2 = CalcDistanceAtAngle(points, template, radian1);
            }
        }

        return Mathf.Min(tempDistance1, tempDistance2);
    }

    public float CalcDistanceAtAngle(List<Vector2> points, List<Vector2> template, float radians)
    {
        Vector2 center = CalcCenterOfGesture(points);
        return CalcGestureTemplateDistance(RotateGesture(points, radians, center), template);
    }

    public float CalcGestureTemplateDistance(List<Vector2> points, List<Vector2> template)
    {
        float distance = 0;
        for (int a = 0; a < points.Count; ++a)
        {
            distance += CalcDistance(points[a], template[a]);
        }
        return distance / points.Count;
    }
}
