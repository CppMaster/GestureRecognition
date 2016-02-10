using UnityEngine;
using System.Collections.Generic;

public class GestureRecognizerSimplifier : GestureRecognizerDeltaAngle
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
        gestureChosen = GestureMatch(pointArray[2]);
        return gestureChosen;
    }

    public override List<Vector2> Transform(List<Vector2> points)
    {
        return RemoveSmallEdges(RemoveSmallAngles(OptimizeGesture(points, maxPoints)));
    }

    public override int GestureMatch(List<Vector2> points)
    {
        float[] deltaAngles = GetDeltaAngles(points);

        float[][] templateDeltaAngles = new float[GestureTemplates.templates.Length][];
        for (int a = 0; a < GestureTemplates.templates.Length; ++a)
        {
            templateDeltaAngles[a] = GetDeltaAngles(Transform(new List<Vector2>(GestureTemplates.templates[a])));
        }

        float minDiff = Mathf.Infinity;
        int minIndex = -1;

        for (int t = 0; t < GestureTemplates.templates.Length; ++t)
        {
            for (int o = -maxOffset; o <= maxOffset; ++o)
            {
                float diff = 0f;

                for (int a = 0; a < deltaAngles.Length || a < templateDeltaAngles[t].Length; ++a)
                {
                    if (a >= deltaAngles.Length)
                        diff += templateDeltaAngles[t][a] * Mathf.Rad2Deg;
                    else if (a >= templateDeltaAngles[t].Length)
                        diff += deltaAngles[a];
                    else
                        diff += Mathf.Abs(Mathf.DeltaAngle(deltaAngles[a] * Mathf.Rad2Deg, templateDeltaAngles[t][a] * Mathf.Rad2Deg));
                }

                diff /= deltaAngles.Length;

                if (diff < minDiff)
                {
                    minDiff = diff;
                    minIndex = t;
                }
            }

            /*if (bothDirections)
            {
                for (int o = -maxOffset; o <= maxOffset; ++o)
                {
                    float diff = 0f;

                    for (int a = 0; a < deltaAngles.Length; ++a)
                    {
                        float deltaAngle = Mathf.DeltaAngle(deltaAngles[a] * Mathf.Rad2Deg, templateDeltaAngles[deltaAngles.Length - a - 1] * Mathf.Rad2Deg);
                        diff += Mathf.Abs(deltaAngle);
                    }

                    diff /= deltaAngles.Length;

                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        minIndex = t;
                    }
                }
            }

            if (oppositeAngles)
            {
                for (int o = -maxOffset; o <= maxOffset; ++o)
                {
                    float diff = 0f;

                    for (int a = 0; a < deltaAngles.Length; ++a)
                    {
                        float deltaAngle = Mathf.DeltaAngle(deltaAngles[a] * Mathf.Rad2Deg, -templateDeltaAngles[a] * Mathf.Rad2Deg);
                        diff += Mathf.Abs(deltaAngle);
                    }

                    diff /= deltaAngles.Length;

                    if (diff < minDiff)
                    {
                        minDiff = diff;
                        minIndex = t;
                    }
                }

                if (bothDirections)
                {
                    for (int o = -maxOffset; o <= maxOffset; ++o)
                    {
                        float diff = 0f;

                        for (int a = 0; a < deltaAngles.Length; ++a)
                        {
                            float deltaAngle = Mathf.DeltaAngle(deltaAngles[a] * Mathf.Rad2Deg, -templateDeltaAngles[deltaAngles.Length - a - 1] * Mathf.Rad2Deg);
                            diff += Mathf.Abs(deltaAngle);
                        }

                        diff /= deltaAngles.Length;

                        if (diff < minDiff)
                        {
                            minDiff = diff;
                            minIndex = t;
                        }
                    }
                }
            } */

        }

        Debug.Log("Result(a): " + GestureTemplates.templateNames[minIndex] + " [" + minIndex + "], Score:" + minDiff);

        return minIndex;
    }
}
