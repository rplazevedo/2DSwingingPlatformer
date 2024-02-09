using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static Vector2 TransformPoint2D(this Transform transform, Vector3 point)
        {
            var transformedPoint = transform.TransformPoint(point);
            return (Vector2)transformedPoint;
        }
    }
}
