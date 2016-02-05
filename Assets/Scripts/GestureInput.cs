using UnityEngine;
using System.Collections.Generic;

public class GestureInput : MonoBehaviour
{

    public GestureRecognizer regonizer;
    public DrawGesture drawGesture;

    List<Vector2> points = new List<Vector2>();

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            points.Add((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * 0.5f);
        }
        else
        {
            if (points.Count > 0)
            {
                regonizer.StartRecognizer(points);
                if (drawGesture != null)
                    drawGesture.DrawRecognizer();
            }
            points.Clear();
        }
    }
}
