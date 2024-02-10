using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class LineRendererExtensions
    {
        public static void UpdateLinePoints(this LineRenderer lineRenderer, List<Vector3> points)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
