﻿using UnityEngine;
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
            points.Add(Input.mousePosition);
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
