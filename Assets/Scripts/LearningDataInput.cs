using UnityEngine;
using System.Collections.Generic;

public class LearningDataInput : MonoBehaviour
{

    public GestureRecognizerPosition gestureRecognizer;
    public GestureLearning gestureLearning;
    public int gestureID = 0;
    public DrawLine drawLine;

    List<Vector2> points = new List<Vector2>();
    List<Vector2> gesture;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            points.Clear();
        if (Input.GetMouseButton(0))
        {
            points.Add((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * 0.5f);
        }
        if (Input.GetMouseButtonUp(0))
        {
            gesture = gestureRecognizer.OptimizeGesture(points, GestureLearning.pointsCount);
            gesture = gestureRecognizer.NormalizeGesture(gesture);
            if (drawLine) drawLine.SetPoints(gesture);
            Calculate();
        }
    }

    [ContextMenu("Add")]
    public void Add()
    {
        if (points.Count == 0)
            return;

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
                data.input[a] = GestureRecognizerDeltaAngle.NormalizeAngleTo01(deltaAngles[a - GestureLearning.pointsCount * 2]);
        }

        data.output = new float[GestureLearning.gestureCount];
        data.output[gestureID] = 1f;

        gestureLearning.Add(data);
    }

    [ContextMenu("Calculate")]
    public void Calculate()
    {
        if (points.Count == 0)
            return;

        float[] deltaAngles = GestureRecognizerDeltaAngle.GetDeltaAngles(gesture);

        float[] input = new float[GestureLearning.inputSize];
        for (int a = 0; a < input.Length; ++a)
        {
            if (a < GestureLearning.pointsCount)
                input[a] = gesture[a].x;
            else if (a < GestureLearning.pointsCount * 2)
                input[a] = gesture[a - GestureLearning.pointsCount].y;
            else
                input[a] = GestureRecognizerDeltaAngle.NormalizeAngleTo01(deltaAngles[a - GestureLearning.pointsCount * 2]);
        }

        float[] output = gestureLearning.Calculate(input);

        Debug.Log("### Gesture ###");
        for (int a = 0; a < output.Length; ++a)
        {
            Debug.Log("Gesture(" + a + "): " + output[a]);
        }
    }

}
