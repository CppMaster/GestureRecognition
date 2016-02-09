using UnityEngine;
using System.Collections.Generic;

public class LearningDataInput : MonoBehaviour
{

    public GestureRecognizerPosition gestureRecognizer;
    public GestureLearning gestureLearning;
    public int gestureID = 0;

    List<Vector2> points = new List<Vector2>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            points.Clear();
        if (Input.GetMouseButton(0))
        {
            points.Add((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * 0.5f);
        }
    }

    [ContextMenu("Add")]
    public void Add()
    {
        if (points.Count == 0)
            return;

        List<Vector2> gesture = gestureRecognizer.OptimizeGesture(points, GestureLearning.pointsCount);
        gesture = gestureRecognizer.NormalizeGesture(points);
        float[] deltaAngles = GestureRecognizerDeltaAngle.GetDeltaAngles(gesture);

        NeuralNetworkIO data = new NeuralNetworkIO();
        data.input = new float[GestureLearning.inputSize];
        for (int a = 0; a < data.input.Length; ++a)
        {
            if (a < GestureLearning.pointsCount)
                data.input[a] = gesture[a].x;
            else if (a < GestureLearning.pointsCount * 2)
                data.input[a] = gesture[a - GestureLearning.pointsCount].y;
            else
                data.input[a] = deltaAngles[a - GestureLearning.pointsCount * 2];
        }

        data.output = new float[GestureLearning.gestureCount];
        data.output[gestureID] = 1f;
    }

}
