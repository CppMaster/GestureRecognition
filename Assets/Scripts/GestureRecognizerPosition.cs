using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureRecognizerPosition : GestureRecognizer
{

    public float sizeOfScaleRect = 500f;
    public int compareDetail = 15;
    public float angleRange = 45f;

    public override int StartRecognizer(List<Vector2> points)
    {
        pointArray = new List<Vector2>[4];

        pointArray[0] = OptimizeGesture(points, maxPoints);
        Vector2 center = CalcCenterOfGesture(pointArray[0]);
        float radians = Mathf.Atan2(center.y - pointArray[0][0].y, center.x - pointArray[0][0].x);
        pointArray[1] = RotateGesture(pointArray[0], -radians, center);
        pointArray[2] = ScaleGesture(pointArray[1], sizeOfScaleRect);
        pointArray[3] = TranslateGestureToOrigin(pointArray[2]);
        gestureChosen = GestureMatch(pointArray[3]);
        return gestureChosen;
    }

    public List<Vector2> NormalizeGesture(List<Vector2> points)
    {
        Vector2 center = CalcCenterOfGesture(points);
        float radians = Mathf.Atan2(center.y - points[0].y, center.x - points[0].x);
        points = RotateGesture(points, -radians, center);
        points = ScaleGesture(points, sizeOfScaleRect);
        points = TranslateGestureToOrigin(points);
        return ScaleValuesTo01(points);
    }

    public List<Vector2> RotateGesture(List<Vector2> points, float radians, Vector2 center)
    {
        List<Vector2> newArray = new List<Vector2>();
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        for (int i = 0; i < points.Count; ++i)
        {
            newArray.Add(new Vector2(
                (points[i].x - center.x) * cos - (points[i].y - center.y) * sin + center.x,
                (points[i].x - center.x) * sin + (points[i].y - center.y) * cos + center.y
                ));
        }

        return newArray;
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
        float factor = Mathf.Max(boundingBox.width, boundingBox.height);

        List<Vector2> newArray = new List<Vector2>();

        for (int i = 0; i < points.Count; ++i)
        {
            newArray.Add(new Vector2(
                points[i].x * (size / factor),
                points[i].y * (size / factor)
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

    public List<Vector2> ScaleValuesTo01(List<Vector2> points)
    {
        List<Vector2> newArray = new List<Vector2>();

        for (int i = 0; i < points.Count; ++i)
        {
            newArray.Add(new Vector2(
                (points[i].x + sizeOfScaleRect / 2) / sizeOfScaleRect,
                (points[i].y + sizeOfScaleRect / 2) / sizeOfScaleRect
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

        Debug.Log("Result(p): " + GestureTemplates.templateNames[index] + " [" + index + "], Score:" + score);
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
