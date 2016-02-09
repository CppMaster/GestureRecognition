using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureRecognizerDeltaAngle : GestureRecognizer
{

    public bool bothDirections = true;
    public bool oppositeAngles = true;
    public int maxOffset = 0;

    public override int StartRecognizer(List<Vector2> points)
    {
        pointArray = new List<Vector2>[1];

        pointArray[0] = OptimizeGesture(points, maxPoints);

        gestureChosen = GestureMatch(pointArray[0]);
        return gestureChosen;
    }

    public int GestureMatch(List<Vector2> points)
    {

        float[] deltaAngles = GetDeltaAngles(points);

        float minDiff = Mathf.Infinity;
        int minIndex = -1;

        for (int t = 0; t < GestureTemplates.templates.Length; ++t)
        {
            for (int o = -maxOffset; o <= maxOffset; ++o)
            {
                float diff = 0f;

                float[] templateDeltaAngles = GetDeltaAngles(new List<Vector2>(GestureTemplates.templates[t]), o);

                for (int a = 0; a < deltaAngles.Length; ++a)
                {
                    float deltaAngle = Mathf.DeltaAngle(deltaAngles[a] * Mathf.Rad2Deg, templateDeltaAngles[a] * Mathf.Rad2Deg);
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

                    float[] templateDeltaAngles = GetDeltaAngles(new List<Vector2>(GestureTemplates.templates[t]), o);

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

                    float[] templateDeltaAngles = GetDeltaAngles(new List<Vector2>(GestureTemplates.templates[t]), o);

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

                        float[] templateDeltaAngles = GetDeltaAngles(new List<Vector2>(GestureTemplates.templates[t]), o);

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
            }

        }

        Debug.Log("Result(a): " + GestureTemplates.templateNames[minIndex] + " [" + minIndex + "], Score:" + minDiff);

        return minIndex;
    }

    public static float[] GetRadians(List<Vector2> points)
    {
        float[] radians = new float[points.Count];

        for (int a = 0; a < points.Count; ++a)
        {
            Vector2 translation;
            if (a < points.Count - 1)
                translation = points[a + 1] - points[a];
            else
                translation = points[a] - points[0];

            radians[a] = Mathf.Atan2(translation.y, translation.x);
        }

        return radians;
    }

    public static float[] GetDeltaAngles(List<Vector2> points, int offset = 0)
    {
        float[] radians = GetRadians(points);
        float[] deltaAngles = new float[radians.Length];

        for (int a = 0; a < deltaAngles.Length; ++a)
        {
            deltaAngles[a] = radians[(a + offset + deltaAngles.Length) % deltaAngles.Length] - radians[(a + 1 + offset + deltaAngles.Length) % deltaAngles.Length];
        }

        return deltaAngles;
    }

    public static float NormalizeAngleTo01(float angle)
    {
        if (angle < -Mathf.PI)
            angle += Mathf.PI * 2;
        if (angle > Mathf.PI)
            angle -= Mathf.PI * 2;
        return Mathf.Abs(angle / Mathf.PI);
    }
}
