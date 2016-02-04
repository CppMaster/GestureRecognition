using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureRecognizerDeltaAngle : GestureRecognizer
{

    public override int StartRecognizer(List<Vector2> points)
    {
        pointArray = new List<Vector2>[1];

        pointArray[0] = OptimizeGesture(points, maxPoints);

        gestureChosen = GestureMatch(pointArray[0]);
        return gestureChosen;
    }

    int GestureMatch(List<Vector2> points)
    {

        float[] deltaAngles = GetDeltaAngles(points);

        float minDiff = Mathf.Infinity;
        int minIndex = -1;

        for (int t = 0; t < GestureTemplates.templates.Length; ++t)
        {
            float diff = 0f;

            float[] templateDeltaAngles = GetDeltaAngles(new List<Vector2>(GestureTemplates.templates[t]));

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

        Debug.Log("Result: " + GestureTemplates.templateNames[minIndex] + " [" + minIndex + "], Score:" + minDiff);

        return minIndex;
    }

    float[] GetDeltaAngles(List<Vector2> points)
    {
        float[] radians = new float[points.Count - 1];
        float[] deltaAngles = new float[radians.Length - 1];

        for (int a = 0; a < deltaAngles.Length; ++a)
        {
            Vector2 translation = points[a + 1] - points[a];
            radians[a] = Mathf.Atan2(translation.y, translation.x);
            if (a > 0)
                deltaAngles[a - 1] = radians[a] - radians[a - 1];
        }

        return deltaAngles;
    }
}
