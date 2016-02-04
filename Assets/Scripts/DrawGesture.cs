using UnityEngine;
using System.Collections.Generic;

public class DrawGesture : MonoBehaviour
{

    public DrawLine drawLine;
    public GestureRecognizer recognizer;

    public int templateIndex = 0;
    public int getsureStep = 0;

    [ContextMenu("DrawTemplate")]
    public void DrawTemplate()
    {
        drawLine.SetPoints(new List<Vector2>(GestureTemplates.templates[templateIndex]));
    }

    [ContextMenu("DrawRecognizer")]
    public void DrawRecognizer()
    {
        drawLine.SetPoints(recognizer.GetStepPoints(getsureStep));
    }
}
