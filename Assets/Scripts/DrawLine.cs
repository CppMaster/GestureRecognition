using UnityEngine;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{

    public LineRenderer line;

    public void SetPoints(List<Vector2> _points)
    {
        line.SetVertexCount(_points.Count);
        for (int a = 0; a < _points.Count; ++a)
        {
            line.SetPosition(a, _points[a]);
        }
    }
}
